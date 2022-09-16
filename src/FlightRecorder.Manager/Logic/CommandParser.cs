using System;
using FlightRecorder.Manager.Entities;

namespace FlightRecorder.Manager.Logic
{
    public class CommandParser
    {
        // The index into this array is one of the values from the OperationType
        // enumeration, mapping the operation to the required argument count
        private readonly int[] _requiredArgumentCount = { 3, 3, 2, 2, 2, 1 };

        /// <summary>
        /// Parse the command line, extracting the operation to be performed
        /// and its parameters 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Operation ParseCommandLine(string[] args)
        {
            Operation op = new Operation();

            // Attempt to parse out the operation type from the first argument
            if ((args.Length > 0) && (Enum.TryParse<OperationType>(args[0], out OperationType type)))
            {
                // Check there are sufficient arguments for this operation
                op.Type = type;
                int requiredArgumentCount = _requiredArgumentCount[(int)type];

                // All is OK at this point if the argument count is correct
                op.Valid = (args.Length == requiredArgumentCount);
                if (op.Valid)
                {
                    // Extract the arguments
                    AssignOperationParameters(op, args);
                }
            }

            return op;
        }

        /// <summary>
        /// For those operations that require it, assign the operation parameters
        /// from the command line, based on the operation type
        /// </summary>
        /// <param name="op"></param>
        /// <param name="args"></param>
        private void AssignOperationParameters(Operation op, string[] args)
        {
            switch (op.Type)
            {
                case OperationType.setpassword:
                case OperationType.add:
                    op.UserName = args[1];
                    op.Password = args[2];
                    break;
                case OperationType.delete:
                    op.UserName = args[1];
                    break;
                    break;
                case OperationType.import:
                    op.FileName = args[1];
                    break;
                case OperationType.export:
                    op.FileName = args[1];
                    break;
                default:
                    break;
            }
        }
    }
}
