using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TimeTraveller.ClientModel;

namespace TimeTraveller.ClientModelCreator
{
    class Program
    {
        private static string HOST_BASE_URI = "http://localhost:8080/";
        private static string HOST_RELATIVE_URI = "its";
        private static string USERNAME = "fzwarteveen@gmail.com";

        private class ModelSpec
        {
            public Func<List<Specification>> InitDelegate { get; set; }
            public string Directory {get; set; }
        }

        private static Dictionary<char, ModelSpec> _models = new Dictionary<char, ModelSpec>()
        {
            {'1', new ModelSpec { InitDelegate = InitAutoModel, Directory = @"E:\Git\TTDB\TestModels\Automobile\scripts" } },
            {'2', new ModelSpec { InitDelegate = InitBloggerModel, Directory = @"E:\Git\TTDB\TestModels\BlogModel" }  },
            {'3', new ModelSpec { InitDelegate = InitSimpleModel, Directory = @"E:\Git\TTDB\TestModels\SimpleModel" }  },
            {'4', new ModelSpec { InitDelegate = InitExtendenModel, Directory = @"E:\Git\TTDB\TestModels\ExtendedModel" }  },
        };

        static void Main(string[] args)
        {
            while (true)
            {
                var choice = GetUserChoice();
                if (choice == 'q') 
                    break;
                if (choice >= '1' && choice <= '4')
                {
                    // valid menu choice
                    var specifications = InitialiseModel(choice);

                    PutSpecificationsToTimeTraveller(specifications, _models[choice].Directory);

                    Console.WriteLine("Any key for menu, q to quit");
                    if (Console.ReadKey().KeyChar == 'q')
                        break;
                }
            }
        }

        private static char GetUserChoice()
        {
            Console.Clear();
            Console.WriteLine("Make a choice:");
            Console.WriteLine("\t1 = Car model");
            Console.WriteLine("\t2 = Blogger model");
            Console.WriteLine("\t3 = Simple Person model");
            Console.WriteLine("\t4 = Person and House model");
            Console.WriteLine("");
            Console.WriteLine("\tq = quit");

            return Console.ReadKey().KeyChar;
        }

        private static List<Specification> InitialiseModel(char choice)
        {
            return _models[choice].InitDelegate();
        }

        private static void PutSpecificationsToTimeTraveller(List<Specification> specifications, string basePath)
        {
            specifications.ForEach(specification =>
            {
                var filePath = string.Format(@"{0}\{1}", basePath, specification.Filename);
                byte[] content = File.ReadAllBytes(filePath);

                Console.WriteLine("PUT-ting: " + specification.Name + " from file: " + filePath + " to path:" + specification.Path);

                var response = MakeRequest(specification.Path, content);

                // don't need to see the result
                //Console.WriteLine("Response=" + response);
            });
        }

        private static List<Specification> InitAutoModel()
        {
            List<Specification> specifications = new List<Specification>();

            var objectModel = new ObjectModel("Automobile", "Automobile.xml"); 
            var caseFileSpec = new CaseFileSpecification("Autohistorie", "Autohistorie.xml", objectModel);
            specifications.Add(objectModel);
            specifications.Add(caseFileSpec);
            specifications.Add(new RuleSpec("EnergyLabel", "EnergyLabelRule.xml", objectModel, caseFileSpec));
            specifications.Add(new CaseFile("09-SK-DG", "09SKDG.xml", objectModel, caseFileSpec));
            specifications.Add(new CaseFile("52-RV-XT", "52RVXT.xml", objectModel, caseFileSpec));

            return specifications;
        }

        private static List<Specification> InitBloggerModel()
        {
            List<Specification> specifications = new List<Specification>();

            var objectModel = new ObjectModel("BlogModel", "ObjectModel.xml");
            var caseFileSpec = new CaseFileSpecification("Blogs", "CaseFileSpecBlogger.xml", objectModel);
            specifications.Add(objectModel);
            specifications.Add(caseFileSpec);
            specifications.Add(new CaseFile("MyBlog", "firstdata.xml", objectModel, caseFileSpec));

            return specifications;
        }

        private static List<Specification> InitSimpleModel()
        {
            List<Specification> specifications = new List<Specification>();

            var objectModel = new ObjectModel("SimpleModel", "ObjectModel.xml");
            var caseFileSpec = new CaseFileSpecification("Persoon", "CaseFileSpecPersoon.xml", objectModel);
            specifications.Add(objectModel);
            specifications.Add(caseFileSpec);
            specifications.Add(new RuleSpec("BerekenLeeftijd", "BerekenLeeftijdRule.xml", objectModel, caseFileSpec));
            specifications.Add(new CaseFile("BSN001", "PersoonFloris.xml", objectModel, caseFileSpec));
            specifications.Add(new CaseFile("BSN002", "PersoonRichard.xml", objectModel, caseFileSpec));
            specifications.Add(new CaseFile("BSN003", "PersoonTheoTebockel.xml", objectModel, caseFileSpec));

            return specifications;
        }

        private static List<Specification> InitExtendenModel()
        {
            List<Specification> specifications = new List<Specification>();

            var objectModel = new ObjectModel("ExtendedModel", "ObjectModel.xml");
            var caseFileSpecHuisEigenaar = new CaseFileSpecification("HuisEigenaar", "CaseFileSpecHuisEigenaar.xml", objectModel);
            var caseFileSpecPersoon = new CaseFileSpecification("Persoon", "CaseFileSpecPersoon.xml", objectModel);
            var caseFileSpecWoning = new CaseFileSpecification("Woning", "CaseFileSpecWoning.xml", objectModel);
            specifications.Add(objectModel);
            specifications.Add(caseFileSpecHuisEigenaar);
            specifications.Add(caseFileSpecPersoon);
            specifications.Add(caseFileSpecWoning);
            specifications.Add(new CaseFile("BSN0003", "PersoonGerritGerritsen.xml", objectModel, caseFileSpecPersoon));
            specifications.Add(new CaseFile("BSN0001", "PersoonJanJansen.xml", objectModel, caseFileSpecPersoon));
            specifications.Add(new ViewSpec("PersoonKort", "ViewPersoonSamenvatting.xml", objectModel, caseFileSpecPersoon));
            specifications.Add(new ViewSpec("PersoonLang", "ViewPersoonUitgebreid.xml", objectModel, caseFileSpecPersoon));

            return specifications;
        }

        public static string MakeRequest(string path, byte[] content)
        {
            var uri = new Uri(new Uri(HOST_BASE_URI), string.Format("{0}/{1}", HOST_RELATIVE_URI, path));

            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");
            webClient.Headers.Add("From", USERNAME);

            byte[] resultBuffer = webClient.UploadData(uri, "PUT", content);
            string result = Encoding.UTF8.GetString(resultBuffer);

            return result;
        }

    }
}
