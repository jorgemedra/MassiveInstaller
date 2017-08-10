using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaletteInstaller.utils;
using System.Text.RegularExpressions;
using log4net;

namespace PaletteInstaller
{
    class Core
    {
        private static readonly ILog log = LogManager.GetLogger("Core");

        private string site;
        private string host;

        private string CurVersion;
        private string LastVersion;

        private string URL_VERSION = "http://{0}/api/Installer/GET/CurrentVersion?site={1}";
        private string URL_SCRIPT = "http://{0}/api/Installer/GET/Script?site={1}&Type={2}";
        private string URL_FILE = "http://{0}/api/Installer/GET/Files?site={1}&Type={2}";

        private string TEM_DIR = ".\\TEMP";
        private string TEM_FILE = "PACKAGE.ZIP";

        //TODO: ADD Log4Net 

        public Core(string site, string host)
        {
            CurVersion = "";
            LastVersion = "";
            this.site = site;
            this.host = host;
        }

        public void DoReview()
        {
            char type = '-';
            
            LastVersion = checkLocalVersion();
            CurVersion = getCurrentVersion();

            if (LastVersion.Equals("0")) // 0 significa que no hay paleta instalada.
                type = 'I'; //Make a new installation
            else
            {
                if(!LastVersion.Equals(CurVersion))
                    type = 'U'; //Make an update
            }

            if (type != '-')
            {
                log.Info("--------------------------------------------------------------------");

                if (type != '-')
                    log.Info("Se realizara una nueva instalacion en la Version [" + CurVersion + "]");
                else
                    log.Info("Se realizara una actualizacion de la Version [" + LastVersion + "]  a la Version [" + CurVersion + "] ");

                log.Info("--------------------------------------------------------------------");

                StringBuilder script = getScript(type);
                
                if (Directory.Exists(TEM_DIR))
                    deleteDirectory(TEM_DIR);


                if (createDirectory(TEM_DIR))
                {
                    if (getFile(type, TEM_DIR + "\\" + TEM_FILE))
                        log.Error("El archivio PACKAGE.ZIP fue descargado.");
                    else
                    {
                        log.Error("No fue posible descargar el archivio PACKAGE.ZIP.");
                        return;
                    }
                }//if (createDirectory(TEM_DIR))
                else
                {
                    log.Error("No fue posible crear el directorio: [" + TEM_DIR + "]");
                    return;
                }
                log.Info("Ejecutando de script...");
                if (executeScript(script))
                {
                    log.Info("Script terminado.");
                    updateLocalVersion(CurVersion,LastVersion);

                    if(type == 'I') //Make a new installation
                        log.Info("Los servicios se han instaldo exitosamente a la version: V" + CurVersion);
                    else
                        log.Info("Los servicios se han actualizado exitosamente a la version: V" + CurVersion);
                }
                else
                    log.Error("Fallo la instalacion.");
                deleteDirectory(TEM_DIR);
                

            }
        }

        private string checkLocalVersion()
        {
            RegistryTool rt = new RegistryTool("Palette");
            string v = rt.readString("VERSION");

            if (v.Equals(RegistryTool.DEFAULT))
                return "0";

            return v;
        }

        private void updateLocalVersion(string version, string lversion)
        {
            RegistryTool rt = new RegistryTool("Palette");

            if (!rt.writeString("VERSION", version))
                log.Error("Fallo la actualización de la version en el registry.");
            if (!rt.writeString("LAST_VERSION", lversion))
                log.Error("Fallo la actualización de la version anterior en el registry.");
        }

        private string getCurrentVersion()
        {
            string v = "0";
            string source = string.Format(URL_VERSION, host, site);


            DownloadFile df = new DownloadFile(source, "");
            StringBuilder version = df.DownloadTextFileToMemory();
            v = version.ToString();

            return v;
        }

        private StringBuilder getScript(char type)
        {
            StringBuilder sb = new StringBuilder("");
            string source = string.Format(URL_SCRIPT, host, site, type);


            DownloadFile df = new DownloadFile(source, "");
            sb = df.DownloadTextFileToMemory();
            return sb;

        }

        private bool getFile(char type, string file)
        {
            string source = string.Format(URL_FILE, host, site, type);
            DownloadFile df = new DownloadFile(source, file);
            
            return df.downloadFile();

        }

        private string[] getTokens(string line)
        {
            string[] tokens = new string[2];

            int i = line.IndexOf(' ');

            if (i > 0)
            {
                tokens[0] = line.Substring(0, i);
                tokens[1] = line.Substring(i).Trim();
            }
            else
            {
                tokens[0] = line;
                tokens[1] = "";
            }

            return tokens;
        }

        private string[] parseArguments(string line)
        {
            List<string> list = new List<string>();

            Regex e = new Regex("([^\"|^\\s])+|\"([^\"]+)\"");
            Match m =  e.Match(line);
            
            while(m != null && m.Length > 0)
            //for(int i=0; i< m.Groups.Count; i++)
            {
                list.Add(m.Value.Replace('"',' ').Trim());
                System.Console.WriteLine(m.Value);
                m = m.NextMatch();
            }

            return list.ToArray();
        }

        private void doCommand(string command, string args)
        {
            StringBuilder sb = new StringBuilder("");

            if (args == null)
                args = "";

            sb.Append("$> Ejecutando comando: " + Environment.NewLine);
            sb.Append("\t" + command + " " + args + Environment.NewLine);
            
            string outStr = ExecCommand.Execute(command, args);

            sb.Append("Output: " + Environment.NewLine);
            sb.Append(outStr);
            log.Info(sb.ToString());
        }
        
        private bool createDirectory(string path)
        {
            bool bSuccess = true;

            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    log.Warn("Se creo el directorio: [" + path + "]");
                }
                else
                    log.Warn("El directorio ya existe. Dir: [" + path + "]");
            }
            catch(Exception ex)
            {
                log.Error("Error al crear el directorio: [" + path + "]. Error: " + ex.Message);
                bSuccess = false;
            }

            return bSuccess;
        }

        private bool deleteDirectory(string path)
        {
            bool bError = false;

            try
            {
                Directory.Delete(path,true);
                log.Info("Directorio eliminado: [" + path + "]");
            }
            catch (Exception ex)
            {  
                log.Error("Error al eliminar el directorio: [" + path + "]. Error: " + ex.Message);
                bError = true;
            }

            return bError;
        }

        private bool zipAction(string arguments, bool unzip = false)
        {
            bool result = true;

            string[] args = parseArguments(arguments);

            if(args != null && args.Length == 2)
            {
                ZipTool zt = new ZipTool();

                if (unzip)
                {
                    log.Info("$> Extrallendo contenido del archivo ZIP: [" + args[0] + "] al destino: [" + args[1] + "]");
                    result = zt.UnZipFile(args[0], args[1]);
                }
                else
                {
                    log.Info("$> Creando archivo ZIP: [" + args[1] + "] del Directorio: [" + args[0] + "]");
                    result = zt.CreateZipFileFromDirectory(args[0], args[1], true);
                }
            }

            return result;
        }

        private bool createShortCut(string arguments)
        {
            bool result = true;

            string[] args = parseArguments(arguments);

            if (args != null)
            {
                ShortCutTool st = new ShortCutTool();

                st.CreateShortcut(args[0], args[1], args[2], args[3]);
            }

            return result;
        }

        private bool executeScript(StringBuilder script)
        {
            StringReader sr = new StringReader(script.ToString());
            bool bSuccess = true;
            string line;
            int  stepCount = 0;

            do
            {
                line = sr.ReadLine();
                if (line != null)
                {
                    line = line.Trim();
                    if (line.Length > 0 && line.ElementAt(0) != '#')
                    {
                        stepCount++;
                        StringBuilder sb = new StringBuilder("::Instruccion (" + stepCount + ")::");
                        
                        line = line.Replace("{LST_VER}", "{0}");
                        line = line.Replace("{NEW_VER}", "{1}");
                        line = string.Format(line, LastVersion, CurVersion);

                        sb.Append(Environment.NewLine);
                        sb.Append("\t{" + line + "}");
                        log.Info(sb.ToString());

                        string[] token = getTokens(line);

                        if (!token[1].Equals(""))
                            token[0] = token[0].ToUpper();
                        
                        if (token[0].ElementAt(0) == '$') //Terminal Console Command
                        {
                            token[0] = token[0].Substring(1); //Remove '$' character.
                            if(token.Length >1)
                                doCommand(token[0], token[1]);
                            else
                                doCommand(token[0], null);
                        }
                        else // Specific Command
                        {
                            if (token[0].Equals("MKDIR"))
                                bSuccess = createDirectory(token[1]);
                            if (token[0].Equals("RMDIR"))
                                bSuccess = deleteDirectory(token[1]);
                            else if (token[0].Equals("ZIP"))
                                bSuccess = zipAction(token[1]);
                            else if (token[0].Equals("UNZIP"))
                                bSuccess = zipAction(token[1], true);
                            else if (token[0].Equals("SHORTCUT"))
                                bSuccess = createShortCut(token[1]);
                        } //else // Specific Command
                    }
                }
            } while (bSuccess && line != null);

            return bSuccess;
        }

    }

}
