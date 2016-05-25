using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QSlice {
    static class Serializer {
        public static void Save<T>(string filename, T value) {
            using(var stm = File.Create(filename)) {
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(stm, value);
            }
        }

        public static T Load<T>(string filename) {
            try {
                using(var stm = File.OpenRead(filename)) {
                    var serializer = new DataContractSerializer(typeof(T));
                    var value = (T)serializer.ReadObject(stm);
                    return value;
                }
            }
            catch {
                return default(T);
            }
        }
    }
}
