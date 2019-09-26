using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Generators;

namespace Faker
{
    public class Faker : IFaker
    {

        public Faker()
        {
            _generators = LoadGenerators();
            LoadGeneratorsFromDirectory();    
        }
        
        private readonly Dictionary<Type, IGenerator> _generators;

        private readonly string _pluginPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");

        private Dictionary<Type, IGenerator> LoadGenerators()
        {
            Dictionary<Type, IGenerator> generators = new Dictionary<Type, IGenerator>();
            var currentAssembly = Assembly.GetExecutingAssembly();
            var types = currentAssembly.GetTypes().Where(type =>
                type.GetInterfaces().Any(i => i.FullName == typeof(IGenerator).FullName));
            foreach (var type in types)
            {
                if (type.FullName != null && currentAssembly.CreateInstance(type.FullName) is IGenerator plugin)
                {
                    var generatorType = plugin.GetGenerationType();
                    generators.Add(generatorType, plugin);
                }
            }

            return generators;
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
                var types = assembly.GetTypes().Where(plugin =>
                    plugin.GetInterfaces().Any(i => i.FullName == typeof(IGenerator).FullName));

                foreach (var type in types)
                {
                    if (type.FullName != null && assembly.CreateInstance(type.FullName) is IGenerator plugin)
                    {
                        var generatorType = plugin.GetGenerationType();
                        _generators.Add(generatorType, plugin);
                    }
                }
            }
        }

        private object GenerateValue(Type type)
        {
            if (_generators.TryGetValue(type, out var generator))
            {
                return generator.Generate();
            }
            
            return null;
        }

        public object Create<T>()
        {
            var type = typeof(T);
            
            if (type.IsAbstract)
            {
                throw new ArgumentException("Can't create an instance of abstract class");
            }
            
            var instance = InitializeWithConstructor(type);
            if (instance == null)
            {
                instance = Activator.CreateInstance(type);
            }
            if (_generators.TryGetValue(type, out var generator))
            {
                instance = generator.Generate();
            }
            else if (type.IsValueType)
            {
                FillObject(instance);
            }

            return Convert.ChangeType(instance, type);
        }

        private void FillObject(object instance)
        {
            var type = instance.GetType();
            var properties = new List<PropertyInfo>(type.GetProperties());
            foreach (var property in properties)
            {
                if (!property.CanWrite) continue;
                
                if (_generators.TryGetValue(property.PropertyType, out var generator))
                {
                    property.SetValue(instance, generator.Generate());
                }
            }
            var fieldInfos = new List<FieldInfo>(type.GetFields());
            foreach (var fieldInfo in fieldInfos)
            {
                if (fieldInfo.IsLiteral) continue;
                
                if (_generators.TryGetValue(fieldInfo.FieldType, out var generator))
                {
                    fieldInfo.SetValue(instance, generator.Generate());    
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
            if (constructorInfo != null)
            {
                var constructorParameters = new List<object>();
                var parametersInfo = constructorInfo.GetParameters();
                foreach (var parameterInfo in parametersInfo)
                {
                    var parameterType = parameterInfo.ParameterType;
                    var parameter = GenerateValue(parameterType);
                    constructorParameters.Add(parameter);
                }
                var instance = constructorInfo.Invoke(constructorParameters.ToArray());

                return instance;
            }
            
            return null;
        }
    }
}