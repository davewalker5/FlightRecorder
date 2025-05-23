﻿namespace FlightRecorder.Manager.Entities
{
    public class Operation
    {
        public bool Valid { get; set; }
        public OperationType Type { get; set; }
        public CommandLineEntityType EntityType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FileName { get; set; }
        public string Identifier { get; set; }
    }
}
