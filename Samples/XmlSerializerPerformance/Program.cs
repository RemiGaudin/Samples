using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace XmlSerializerPerformance
{
    static class Program
    {
        static void Main(string[] args)
        {
            // Code from https://www.damirscorner.com/blog/posts/20110612-XmlSerializerConstructorPerformanceIssues.html
            // See also https://kalapos.net/Blog/ShowPost/how-the-evil-system-xml-serialization-xmlserializer-class-can-bring-a-server-with-32gb-ram-down
            // And https://docs.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmlserializer?redirectedfrom=MSDN&view=netframework-4.8#dynamically-generated-assemblies
            TestXmlSerializer();
            TestXmlHelper();
        }
        static void TestXmlSerializer()
        {
            var writer = new StringWriter();
            var xmlWriter = new XmlTextWriter(writer);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Root");
            var stopwatch = new Stopwatch();
            int iterations = 10000;

            stopwatch.Restart();
            // This will take around 35 ms
            for (int i = 0; i < iterations; i++)
            {
                var serializer = new XmlSerializer(typeof(string));
                serializer.Serialize(xmlWriter, "Test");
            }
            stopwatch.Stop();
            Console.WriteLine("{0} serializations with XmlSerializer(string): {1,5} ms", iterations, stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            // This will take around 28000 ms and consume more than 400 MB of RAM!!!
            for (int i = 0; i < iterations; i++)
            {
                var serializer = new XmlSerializer(typeof(string), new XmlRootAttribute("Type"));
                serializer.Serialize(xmlWriter, "Test");
            }
            stopwatch.Stop();
            Console.WriteLine("{0} serializations with XmlSerializer(string, XmlRootAttribute): {1,5} ms", iterations, stopwatch.ElapsedMilliseconds);

            var staticSerializer = new XmlSerializer(typeof(string), new XmlRootAttribute("Type"));
            stopwatch.Restart();
            // This will take around 8 ms
            for (int i = 0; i < iterations; i++)
            {
                staticSerializer.Serialize(xmlWriter, "Test");
            }
            stopwatch.Stop();
            Console.WriteLine("{0} serializations with static XmlSerializer(string, XmlRootAttribute): {1,5} ms", iterations, stopwatch.ElapsedMilliseconds);

            Console.ReadKey();
        }
        static void TestXmlHelper()
        {
            var writer = new StringWriter();
            var xmlWriter = new XmlTextWriter(writer);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Root");
            var stopwatch = new Stopwatch();
            int iterations = 1000000;

            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                var serializer = new XmlSerializer(typeof(string));
                serializer.Serialize(xmlWriter, "Test");
            }
            stopwatch.Stop();
            Console.WriteLine("{0} serializations with XmlSerializer(string): {1,5} ms", iterations, stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            var staticSerial = new XmlSerializer(typeof(string));
            for (int i = 0; i < iterations; i++)
            {
                staticSerial.Serialize(xmlWriter, "Test");
            }
            stopwatch.Stop();
            Console.WriteLine("{0} serializations with static XmlSerializer(string): {1,5} ms", iterations, stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                XmlHelper1.GetSerializer<string>().Serialize(xmlWriter, "Test");
            }
            stopwatch.Stop();
            Console.WriteLine("{0} serializations with XmlHelper1.GetSerializer<string>(): {1,5} ms", iterations, stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                XmlHelper2.GetSerializer<string>().Serialize(xmlWriter, "Test");
            }
            stopwatch.Stop();
            Console.WriteLine("{0} serializations with XmlHelper2.GetSerializer<string>(): {1,5} ms", iterations, stopwatch.ElapsedMilliseconds);
        }

        public static class XmlHelper1
        {
            private static Dictionary<Type, XmlSerializer> _serializers = new Dictionary<Type, XmlSerializer>();
            public static XmlSerializer GetSerializer<T>()
            {
                // Note: XmlSerializer is thread safe: https://docs.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmlserializer?redirectedfrom=MSDN&view=netframework-4.8#thread-safety
                lock (_serializers)
                {
                    Type t = typeof(T);
                    if (!_serializers.ContainsKey(t))
                    {
                        _serializers.Add(t, new XmlSerializer(t));
                    }
                    return _serializers[t];
                }
            }
        }

        public static class XmlHelper2
        {
            private static ConcurrentDictionary<Type, XmlSerializer> _serializers = new ConcurrentDictionary<Type, XmlSerializer>();
            public static XmlSerializer GetSerializer<T>()
            {
                // Note: XmlSerializer is thread safe: https://docs.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmlserializer?redirectedfrom=MSDN&view=netframework-4.8#thread-safety
                return _serializers.GetOrAdd(typeof(T), t => new XmlSerializer(t));
            }
        }
    }
}
