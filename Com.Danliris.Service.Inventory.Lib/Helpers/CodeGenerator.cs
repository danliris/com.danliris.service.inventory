using MlkPwgen;

namespace Com.Danliris.Service.Inventory.Lib.Helpers
{
    public class CodeGenerator
    {
        private const int LENGTH = 8;
        private const string ALLOWED_CHARACTER = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";

        public string GenerateCode()
        {
            return PasswordGenerator.Generate(length: LENGTH, allowed: ALLOWED_CHARACTER);
        }
    }
}
