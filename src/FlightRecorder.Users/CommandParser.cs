using System;

namespace FlightRecorder.Users
{
    public class CommandParser
    {
        // The index into this array is one of the values from the OperationType
        // enumeration, mapping the operation to the required argument count
        private readonly int[] _requiredArgumentCount = { 3, 3, 2 };

        public OperationType Operation { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Validate the command line, extracting the operaiton to be performed
        /// and its arguments 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool ValidateCommandLine(string[] args)
        {
            bool validationResult = false;

            if (args.Length > 0)
            {
                // Attempt to parse out the operation type from the first argument
                if (Enum.TryParse<OperationType>(args[0], out OperationType result))
                {
                    // Check there are sufficient arguments for this operation
                    Operation = result;
                    int requiredArgumentCount = _requiredArgumentCount[(int)result];

                    // All is OK at this point if the argument count is correct
                    validationResult = (args.Length == requiredArgumentCount);
                    if (validationResult)
                    {
                        // Extract the arguments
                        UserName = args[1];
                        if (result != OperationType.delete)
                        {
                            Password = args[2];
                        }
                    }
                }
            }

            return validationResult;
        }
    }
}
