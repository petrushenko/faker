using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Faker
{
    public class Faker : IFaker
    {

        public Faker()
        {
            _config = new FakerConfig();
            _generators = new Dictionary<Type, IGenerator>();
            _genericGeneratorFactory = new List<IGenericGeneratorFactory>();
            LoadGenerators();
            LoadGeneratorsFromDirectory();    
        }

        public Faker(FakerConfig config)
        {
            _config = config;
            _generators = new Dictionary<Type, IGenerator>();
            _genericGeneratorFactory = new List<IGenericGeneratorFactory>();
            LoadGenerators();
            LoadGeneratorsFromDirectory();
        }
        
        private readonly Dictionary<Type, IGenerator> _generators;
        private readonly List<IGenericGeneratorFactory> _genericGeneratorFactory;
        private readonly string _pluginPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
        private readonly FakerConfig _config;

        private void LoadGenerators()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var types = currentAssembly.GetTypes().Where(type =>
                type.GetInterfaces().Any(i => i.FullName == typeof(IGenerator).FullName 
                                              || i.FullName == typeof(IGenericGeneratorFactory).FullName));
            foreach (var type in types)
            {
                if (type.FullName == null) continue;
                if (type.GetInterfaces().Contains(typeof(IGenericGenerator))) continue;
                if (!type.IsClass) continue;
                if (currentAssembly.CreateInstance(type.FullName) is IGenerator generatorPlugin)
                {
                    var generatorType = generatorPlugin.GetGenerationType();
                    _generators.Add(generatorType, generatorPlugin);
                }
                else if(currentAssembly.CreateInstance(type.FullName) is IGenericGeneratorFactory generatorFactoryPlugin)
                {
                    _genericGeneratorFactory.Add(generatorFactoryPlugin);
                }
            }
        }

        private void LoadGeneratorsFromDirectory()
        {
            if (_generators == null) return;
            
            var pluginDirectory = new DirectoryInfo(_pluginPath);
            if (!pluginDirectory.Exists)
            {
                pluginDirectory.Create();
                return;
            }

            var pluginFiles = Directory.GetFiles(pluginDirectory.FullName,"*.dll");

            foreach (var pluginFile in pluginFiles)
            {
                var assembly = Assembly.LoadFrom(pluginFile);
                var types = assembly.GetTypes().Where(type =>
                    type.GetInterfaces().Any(i => i.FullName == typeof(IGenerator).FullName 
                                                  || i.FullName == typeof(IGenericGeneratorFactory).FullName));
                foreach (var type in types)
                {
                    if (type.FullName == null) continue;
                    if (type.GetInterfaces().Contains(typeof(IGenericGenerator))) continue;
                    if (!type.IsClass) continue;
                    if (assembly.CreateInstance(type.FullName) is IGenerator generatorPlugin)
                    {
                        var generatorType = generatorPlugin.GetGenerationType();
                        _generators.Add(generatorType, generatorPlugin);
                    }
                    else if(assembly.CreateInstance(type.FullName) is IGenericGeneratorFactory generatorFactoryPlugin)
                    {
                        _genericGeneratorFactory.Add(generatorFactoryPlugin);
                    }
                }
            }
        }

        private object GenerateValue(Type type, bool currentClass)
        {
            if (_generators.TryGetValue(type, out var generator))
            {
                return generator.Generate();
            }

            if (currentClass) return null;
            if (type.IsValueType || type.IsClass)
            {
                var method = typeof(Faker).GetMethod("Create");
                if (method == null) return null;
                var genericMethod = method.MakeGenericMethod(type);
                return genericMethod.Invoke(new Faker(), null);
            }
            return null;
        }

        public object Create<T>()
        {
            var type = typeof(T);
            
            if (type.IsAbstract)
            {
                return null;
            }
            if (_generators.TryGetValue(type, out var generator))
            {
                return generator.Generate();
            }
            if (type.IsGenericType || type.IsArray || type.IsEnum)
            {
                var generators = CreateGenericGenerators(type);
                if (type.IsArray || type.IsEnum) type = type.BaseType;
                if (type != null && generators.TryGetValue(type, out var genericGenerator))
                {
                    return genericGenerator.Generate();
                }
            }
            else if (type.IsValueType || type.IsClass)
            {
                var instance = InitializeWithConstructor(type);
                FillObject(instance);
                return instance;
            }

            return type != null ? Activator.CreateInstance(type) : null;
        }

        private Dictionary<Type, IGenerator> CreateGenericGenerators(Type type)
        {
            var generators = new Dictionary<Type, IGenerator>();
            foreach (var generatorFactory in _genericGeneratorFactory)
            {
                IGenerator generator;
                if (type.IsGenericType)
                {
                    generator = generatorFactory.GetGenerator(type.GetGenericArguments());
                }
                else if(type.GetElementType() != null)
                {
                    generator = generatorFactory.GetGenerator(new []{ type.GetElementType() });
                }
                else
                {
                    generator = generatorFactory.GetGenerator(new[] { type });
                }
                var generatorType = generator.GetGenerationType();
                if (!generators.ContainsKey(generatorType))
                {
                    generators.Add(generatorType, generator);
                }
            }

            return generators;
        }

        private void FillObject(object instance)
        {
            var type = instance.GetType();
            
            var properties = new List<PropertyInfo>(type.GetProperties());
            foreach (var property in properties)
            {
                if (!property.CanWrite) continue;
                var generator = _config.GetGeneratorByMemberInfo(property);
                if (generator != null)
                {
                    property.SetValue(instance, generator.Generate());
                }
                else
                {
                    var propertyType = property.PropertyType;
                    var value = GenerateValue(propertyType, propertyType == instance.GetType());
                    property.SetValue(instance, value);
                }
            }
            var fieldInfos = new List<FieldInfo>(type.GetFields());
            foreach (var fieldInfo in fieldInfos)
            {
                if (fieldInfo.IsLiteral) continue;
                var generator = _config.GetGeneratorByMemberInfo(fieldInfo);
                if (generator != null)
                {
                    fieldInfo.SetValue(instance, generator.Generate());
                }
                else
                {
                    var fieldType = fieldInfo.FieldType;
                    var value = GenerateValue(fieldType, fieldType == instance.GetType());
                    fieldInfo.SetValue(instance, value);
                }
            }
        }

        private ConstructorInfo GetConstructorWithMaxNumberOfParameters(ConstructorInfo[] constructors)
        {
            if (constructors == null || constructors.Length <= 0) return null;
            
            var constructorInfo = constructors[0];
            foreach (var constructor in constructors)
            {
                if (constructor.GetParameters().Length > constructorInfo.GetParameters().Length)
                {
                    constructorInfo = constructor;
                }
            }

            return constructorInfo;
        }

        private object InitializeWithConstructor(Type type)
        {
            var constructorInfos = type.GetConstructors();
            var constructorInfo = GetConstructorWithMaxNumberOfParameters(constructorInfos);
            if (constructorInfo == null) return Activator.CreateInstance(type);
            
            var constructorParameters = new List<object>();
            var parametersInfo = constructorInfo.GetParameters();
            foreach (var parameterInfo in parametersInfo)
            {
                var parameterName = parameterInfo.Name;
                var generator = _config.GetGeneratorByName(parameterName);
                if (generator != null)
                {
                    constructorParameters.Add(generator.Generate());
                }
                else
                {
                    var parameterType = parameterInfo.ParameterType;
                    var parameter = GenerateValue(parameterType, parameterType == type);
                    constructorParameters.Add(parameter);
                }
                
            }
            var instance = constructorInfo.Invoke(constructorParameters.ToArray());

            return instance;
        }
    }
}