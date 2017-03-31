using System;


namespace WindowsFormsApplication1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string filepath = null;
            if (args.Length > 0)
            {
                filepath = args[0];
            }
                Vsndevts_Decompile.Decompiler decompiler = new Vsndevts_Decompile.Decompiler();
                decompiler.Decompile(filepath);
            
        }

    }
}
