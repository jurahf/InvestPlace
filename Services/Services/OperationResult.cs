using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string ErrorText { get; set; }

        public static OperationResult CreateSuccess()
        {
            return new OperationResult() { Success = true };
        }
        
        public static OperationResult CreateFail(string error)
        {
            return new OperationResult() { Success = false, ErrorText = error };
        }
    }
}
