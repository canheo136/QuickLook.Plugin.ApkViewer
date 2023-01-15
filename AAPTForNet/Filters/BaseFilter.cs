using AAPTForNet.Models;

namespace AAPTForNet.Filters {
    internal abstract class BaseFilter {
        protected const char seperator = '\'';
        protected const string defaultEmptyValue = "Unknown";

        public abstract bool CanHandle(string msg);
        public abstract void AddMessage(string msg);
        public abstract ApkInfo GetAPK();
        /// <summary>
        /// Test in loop
        /// </summary>
        public abstract void Clear();
    }
}
