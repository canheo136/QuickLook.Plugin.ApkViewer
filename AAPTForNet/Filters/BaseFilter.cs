using AAPTForNet.Models;

namespace AAPTForNet.Filters {
    internal abstract class BaseFilter {
        protected const char seperator = '\'';

        public abstract bool canHandle(string msg);
        public abstract void addMessage(string msg);
        public abstract ApkInfo getAPK();
        /// <summary>
        /// Test in loop
        /// </summary>
        public abstract void clear();
    }
}
