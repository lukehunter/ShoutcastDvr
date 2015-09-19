using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace ShoutcastDvr.Model
{
    class Options
    {
        /// <summary>
        /// See HelpText
        /// </summary>
        [Option('s', "schedulepath", Required = false, HelpText = "Specify the json file used to load/save scheduled recordings")]
        public string SchedulePath
        {
            get;
            set;
        }

        private static Options mInstance;

        /// <summary>
        /// Current command line options
        /// </summary>
        public static Options Instance
        {
            get
            {
                return mInstance ?? (mInstance = ParseCommandLine());
            }
        }

        /// <summary>
        /// Build CommandLineOptions object using the command line provided when executing the containing assembly
        /// </summary>
        /// <returns>CommandLineOptions object containing current command line parameters</returns>
        private static Options ParseCommandLine()
        {
            var options = new Options();

            Parser.Default.ParseArguments(Environment.GetCommandLineArgs(), options);

            return options;
        }
    }
}
