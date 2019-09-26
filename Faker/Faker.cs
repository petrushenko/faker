using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Generators;

namespace Faker
{
    public class Faker : IFaker
    {

        public Faker()
        {
            _generators = LoadGenerators();    
        }
        
        private readonly Dictionary<Type, IGenerator> _generators;

        private readonly string _pluginPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");

        private Dictionary<Type, IGenerator> LoadGenerators()
        {
            DirectoryInfo pluginDirectory = new DirectoryInfo(_pluginPath);
            Dictionary<Type, IGenerator> generators = new Dictionary<Type, IGenerator>();
            if (!pluginDirectory.Exists)
            {
                pluginDirectory.Create();
                return generators;
            }

            var pluginFiles = Directory.GetFiles(pluginDirectory.FullName,"*.dll");

            foreach (var pluginFile in pluginFiles)
            {
                Assembly assembly = Assembly.LoadFrom(pluginFile);
                var types = assembly.GetTypes().Where(plugin =>
                    plugin.GetInterfaces().Any(i => i.FullName == typeof(IGenerator).FullName));

                foreach (Type type in types)
                {
                    if (type.FullName != null && assembly.CreateInstance(type.FullName) is IGenerator plugin)
                    {
                        Type generatorType = plugin.GetGenerationType();
                        generators.Add(generatorType, plugin);
                    }
                }
            }

            return generators;
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

            var instance = InitializeWithConstructor(type);

            if (instance != null)
            {
                FillObject(instance);
            }

            return instance;
        }

        private void FillObject(object instance)
        {
            var type = instance.GetType();
            var properties = new List<PropertyInfo>(type.GetProperties());
            foreach (var property in properties)
            {
                if (property.CanWrite)
                {
                    property.SetValue(instance, GenerateValue(property.PropertyType));
                }
            }
            var fieldInfos = new List<FieldInfo>(type.GetFields());
            foreach (var fieldInfo in fieldInfos)
            {
                fieldInfo.SetValue(instance, GenerateValue(fieldInfo.FieldType));
            }
        }

        private ConstructorInfo GetConstructorWithMaxNumberOfParameters(ConstructorInfo[] constructors)
        {
            if (constructors == null || constructors.Length <= 0) return null;
            
            ConstructorInfo constructorInfo = constructors[0];
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
            ConstructorInfo[] constructorInfos = type.GetConstructors();
            var constructorInfo = GetConstructorWithMaxNumberOfParameters(constructorInfos);
            if (constructorInfo != null)
            {
                var constructorParameters = new List<object>();
                var parametersInfo = constructorInfo.GetParameters();
                foreach (var parameterInfo in parametersInfo)
                {
                    var parameterType = parameterInfo.ParameterType;
                    object parameter = GenerateValue(parameterType);
                    constructorParameters.Add(parameter);
                }
                object instance = constructorInfo.Invoke(constructorParameters.ToArray());

                return instance;
            }
            
            return null;
        }
    }
}