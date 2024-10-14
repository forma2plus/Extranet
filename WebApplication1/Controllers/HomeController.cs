using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Odbc;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Helper;
using WebApplication1.Models;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Collections.Specialized;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

//using System.Net.Http.Formatting;
namespace WebApplication1.Controllers
{
    public static class TokenManager
    {
        private static readonly string secretKey = "UneCléDe32OctetsDoitAvoir256BitsDeLongueu1212121212133r"; // Changez ceci par une clé secrète appropriée

        public static string GenerateToken(int id)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("Id", id.ToString())
            }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static int ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
               ValidateLifetime = false, // Désactiver la validation de durée de vie du jeton
               //ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var claims = ((JwtSecurityToken)validatedToken).Claims;
            int id = int.Parse(claims.First(c => c.Type == "Id").Value);

            return id;
        }
    }

    public static class UrlEncryptor
    {
        private const string EncryptionKey = "VotreClefSecrete"; // Changez ceci par une clé secrète appropriée

        public static string Encrypt(int id)
        {
            // Convertissez l'ID en tableau de bytes
            byte[] idBytes = BitConverter.GetBytes(id);

            // Ajoutez votre logique de chiffrement ici
            // Pour simplifier, cet exemple utilise Base64
            byte[] encryptedData = PerformEncryption(idBytes);
            return Convert.ToBase64String(encryptedData);
        }

        public static int Decrypt(string encryptedValue)
        {
            byte[] encryptedData = Convert.FromBase64String(encryptedValue);
            byte[] decryptedData = PerformDecryption(encryptedData);

            // Convertissez le tableau de bytes en entier
            return BitConverter.ToInt32(decryptedData, 0);
        }

        private static byte[] PerformEncryption(byte[] data)
        {
            // Ajoutez votre logique de chiffrement ici
            // Pour simplifier, cet exemple utilise XOR comme opération de chiffrement
            byte[] keyBytes = Encoding.UTF8.GetBytes(EncryptionKey);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= keyBytes[i % keyBytes.Length];
            }
            return data;
        }

        private static byte[] PerformDecryption(byte[] data)
        {
            // Ajoutez votre logique de déchiffrement ici
            // Pour simplifier, cet exemple utilise la même opération que le chiffrement
            return PerformEncryption(data);
        }
    }
    public class HomeController : Controller
    {
        
        OdbcConnection cnn = new OdbcConnection(ConfigurationManager.ConnectionStrings["format"].ToString());
        
        public ActionResult Result()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
        private async Task<string> SendXmlDataAndGetJsonResponse(string apiUrl, string xmlData)
        {
            using (HttpClient client = new HttpClient())
            {
                // Construct the URL with parameters
                string fullUrl = $"{apiUrl}?param1=value1&param2=value2"; // Add your parameters here

                // Assuming your API endpoint expects Content-Type as application/xml
                client.DefaultRequestHeaders.Add("Content-Type", "application/xml");

                // Send the XML data as a POST request
                HttpResponseMessage response = await client.PostAsync(fullUrl, new StringContent(xmlData, Encoding.UTF8, "application/xml"));

                if (response.IsSuccessStatusCode)
                {
                    // Read and return the JSON response
                    return await response.Content.ReadAsStringAsync();
                }

                // Handle non-success status code if needed
                return null;
            }
        }
        [HttpPost]
        public ActionResult GenerateToken(TokenRequestModel model)
        {
            if (ModelState.IsValid)
            {
                // Utilisez l'ID pour générer le token (ex : appel à un service, traitement, etc.)
                string token = GenerateTokenForId(model.Id);

                // Retournez le token à la vue ou faites ce que vous devez faire avec le token
                ViewBag.Token = token;
                return View("TokenResult");
            }

            // Si le modèle n'est pas valide, revenez à la vue du formulaire
            return View("Index", model);
        }

        private string GenerateTokenForId(int id)
        {
            // Logique pour générer le token en fonction de l'ID
            // (à remplacer par votre propre logique)
            return $"Token_for_ID_{id}";
        }

        public ActionResult Plogon_ancien()
        {
            return View();
        }

        public ActionResult Plogon()
        {
            return View();
        }
        public ActionResult Intro()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Result(StagiaireModel models)
        {
          
                // try
                // {
                if (ModelState.IsValid)
                {


                    var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, PROFIL.*, personal_interest.*, stagiaire_profil.numero FROM (stagiaire_profil LEFT JOIN personal_interest ON stagiaire_profil.numero = personal_interest.numero_profil) LEFT JOIN PROFIL ON stagiaire_profil.numero = PROFIL.numero WHERE (((stagiaire_profil.numero)=" + models.Numero + "));");
                    // var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, stagiaire_profil.numero FROM stagiaire_profil WHERE (((stagiaire_profil.numero)="+id+"))");
                    if (obj.Count() == 0) { return RedirectToAction("Index", "Home"); }
                    if (obj != null)
                    {
                        StagiaireModel model = new StagiaireModel();


                        model.mail = obj.FirstOrDefault().mail;
                        model.fonction = obj.FirstOrDefault().fonction;
                        model.Numero = obj.FirstOrDefault().Numero;
                        model.NationInterloc = obj.FirstOrDefault().NationInterloc;
                        model.nom = obj.FirstOrDefault().nom;
                        model.Portable = obj.FirstOrDefault().Portable;
                        model.premlang = obj.FirstOrDefault().premlang;
                        model.prenom = obj.FirstOrDefault().prenom;
                        model.Departement = obj.FirstOrDefault().Departement;
                        model.societe = obj.FirstOrDefault().societe;
                        model.Civilite = obj.FirstOrDefault().Civilite;
                        model.numindividu = obj.FirstOrDefault().numindividu;
                        model.type_mail = obj.FirstOrDefault().type_mail;

                        // var obje = postXMLData( UrlData,Postxml);

                        // return RedirectToAction("Intro", "Home");
                        // string TheUrl = "http://s371880604.onlinehome.fr/webcalendar_salle/srcs/www/auto.php?module=auto&action=import:updateStagiaire";
                        string datas = "&refIndividu=" + model.refindividu + "&mdp=" + model.Numero + "&nom=" + model.nom + "&prenom=" + model.prenom + "";
                        string urls = "http://s371880604.onlinehome.fr/webcalendar_salle/srcs/www/auto.php?module=auto&action=import:updateStagiaire" + "&refIndividu=" + model.numindividu + "&mdp=" + model.Numero + "&nom=" + model.nom + "&prenom=" + model.prenom + "";


                        string StrXMLStagiaire = "<xml><stagiaires><stagiaire>";
                        StrXMLStagiaire = StrXMLStagiaire + "<civilite> " + model.Civilite + "</civilite>";
                        StrXMLStagiaire = StrXMLStagiaire + "<nom>" + model.nom + "</nom>";
                        StrXMLStagiaire = StrXMLStagiaire + "<prenoms>" + model.prenom + "</prenoms>";
                        StrXMLStagiaire = StrXMLStagiaire + "<mail>" + model.mail + "</mail>";
                        StrXMLStagiaire = StrXMLStagiaire + "<fonction>" + model.fonction + "</fonction>";
                        StrXMLStagiaire = StrXMLStagiaire + "<societe>" + model.societe + "</societe>";
                        StrXMLStagiaire = StrXMLStagiaire + "<num_individu>" + model.numindividu + "</num_individu>";
                        StrXMLStagiaire = StrXMLStagiaire + "<telephone>" + model.Telephone + "</telephone>";
                        StrXMLStagiaire = StrXMLStagiaire + "<portable>" + model.Portable + "</portable>";
                        StrXMLStagiaire = StrXMLStagiaire + "<rue_stage>" + model.RueDuStage + "</rue_stage>";
                        StrXMLStagiaire = StrXMLStagiaire + "<ville_stage>" + model.VilleDuStage + "</ville_stage>";
                        StrXMLStagiaire = StrXMLStagiaire + "<Cp_stage>" + model.CPDuStage + "</Cp_stage>";

                        StrXMLStagiaire = StrXMLStagiaire + "</stagiaire> </stagiaires></xml>";
                        //string StrXMLStagiaire = "<xml><stagiaires><stagiaire>";
                        //  StrXMLStagiaire =  "civilite=" + model.Civilite + "&";
                        //StrXMLStagiaire = StrXMLStagiaire + "nom=" + model.Nom + "&";
                        // StrXMLStagiaire = StrXMLStagiaire + "prenoms=" + model.Prenom + "&";
                        //  StrXMLStagiaire = StrXMLStagiaire + "mail=" + model.Mail + "&";
                        //  StrXMLStagiaire = StrXMLStagiaire + "fonction=" + model.Fonction + "&";
                        //  StrXMLStagiaire = StrXMLStagiaire + "societe=" + model.Societe + "&";
                        //  StrXMLStagiaire = StrXMLStagiaire + "num_individu=" + model.numindividu + "&";
                        //  StrXMLStagiaire = StrXMLStagiaire + "telephone=" + model.Telephone +"&";
                        //  StrXMLStagiaire = StrXMLStagiaire + "portable=" + model.Portable +"&";
                        //  StrXMLStagiaire = StrXMLStagiaire + "rue_stage=" + model.RueDuStage +"&";
                        //  StrXMLStagiaire = StrXMLStagiaire + "ville_stage="+ model.VilleDuStage;
                        // StrXMLStagiaire = StrXMLStagiaire + "Cp_stage=" + model.CPDuStage + "&";

                        //StrXMLStagiaire = StrXMLStagiaire + "</stagiaire> </stagiaires></xml>";
                        // string urlpaht = "http://s371880604.onlinehome.fr/webcalendar/srcs/www/index.php?module=auto&action=import:index&PostedXMLStagiaire=";

                        //  string urly = "http://s371880604.onlinehome.fr/webcalendar/srcs/www/index.php?module=auto&action=import:index";
                        // Httpcontent PostedXMLStagiaire = new HttpContent();
                        string PostedXMLStagiaire = StrXMLStagiaire;
                        string urlwebcal = "https://formateur.forma2plus.com/auto.php/webservices/security/authenticate?";
                        string token = GetTokenFromUrl(model.numindividu); // Supposons que cette fonction retourne le jeton
                        string urlWithToken = $"{urlwebcal}token={token}";
                        ViewBag.urlwebcal = urlWithToken;
                        ViewBag.idInd = model.numindividu;
                    ViewBag.type_mail = model.type_mail;
                        // var envoie = InsertIp(model.numindividu);
                        var permission = 0;
                        if (model.type_mail != "P0")
                        {
                            if (model.type_mail == "01")
                            {
                                // var response = GetPHP(urls);
                                //var reponsesPlanifier = getProductLIstXML(urlpaht,PostedXMLStagiaire);

                                //var reponsesPlanifier = GetPHPXML(urly, StrXMLStagiaire);
                                ViewBag.url = urls;
                                //ViewBag.response = response;
                                //ViewBag.urlplan = urlpaht;
                                // ViewBag.responsesPlanifier = reponsesPlanifier;


                                //  sendEmailConfirmationButErreur(email, ref_we, ref_order, amount, ex.Message);
                                // return ViewBag.reponsesPlanifier;

                            }
                            /*  if (model.type_mail == "P1")
                              {
                                 // var response = GetPHP(urls);
                                  var reponsesPlanifier = GetPHP(urlpaht);
                                 // ViewBag.url = urls;
                                 // ViewBag.response = response;
                                  ViewBag.urlplan = urlpaht;
                                  ViewBag.responsesPlanifier = reponsesPlanifier;
                              }*/

                        }
                        List<StagiaireModel> model4 = StagiaireModel.PermssionTel(model.Numero);

                        permission = model4.Count;
                        List<StagiaireModel> model5 = StagiaireModel.PermssionFonction(model.Numero);

                        permission = model5.Count;

                        List<StagiaireModel> model2 = StagiaireModel.Profil(model.Numero);

                        //var obj23= cnn.Execute(@"SELECT test_en_ligne.numero_idbase, test_en_ligne.* FROM test_en_ligne WHERE (((test_en_ligne.numero_idbase)=" + id + "))");

                        if (permission > 0) { permission = model2.Count; }



                        List<StagiaireModel> model3 = StagiaireModel.Permissions(model.Numero);

                        if (permission > 0) { permission = model3.Count; }
                        ViewBag.permision = permission;
                        List<StagiaireModel> model1 = StagiaireModel.Noteglobal(model.Numero);

                        //var obj2 = cnn.Execute(@"SELECT test_en_ligne.numero_idbase, test_en_ligne.* FROM test_en_ligne WHERE (((test_en_ligne.numero_idbase)=" + id + "))");
                        if (model1.Count > 0)
                        {
                            StagiaireModel dbUStagiaireObject = model1.First();
                        }
                        ViewBag.Note_globale = model1.Count;


                        return View(model);


                    }
                    else
                    {
                        ViewBag.ResMessege = ResMessege.getDanger("Cette numero n'exite pas");
                        return View();

                    }

                }
                else
                {

                    return View();

                }


            }
        public void SendEmail(string mail, int Numero,  string nom, string prenom)
        {
            var senderemail = new MailAddress("suividemonstage@forma2plus.com", "Forma2plus");
            var receiveremail = new MailAddress(mail, "Receiver");
           //  var receiveremail = new MailAddress("hermone@forma2plus.com", "Receiver");
            string body;
            var password = "hermone";
            string subject = "Forma2plus -  Envoi de votre code d'identification";
            //  string body = "Bonjour,<br/><br/> Votre profil a été mis à jour, et nous voulions nous assurer que vous avez accès aux informations les plus récentes.<br/><br/>Pour accéder à votre profil mis à jour, veuillez utiliser le lien suivant: https://extranet.forma2plus.com/Profil/Plogon/" + model.Numero+"/"+nom+ ".<br/><br/>Votre Identifiant: " + nom+ "<br/>Votre numéro de dossier : " + model.Numero+ ".<br/><br/>N\'hésitez pas à nous contacter si vous avez des questions ou des préoccupations concernant ces modifications.<br/><br/> Cordialement ";
             body = "Bonjour,\n\nSuite à votre demande, nous vous envoyons votre code d'identification nécessaire pour accéder à vos profil.\nVotre Identifiant: " + mail + "\nVotre numéro de dossier : " + Numero + ".\n\nVeuillez conserver ce code en lieu sûr et ne le partager avec personne pour des raisons de sécurité. Si vous rencontrez le moindre problème ou si vous avez des questions, n'hésitez pas à nous contacter.\n\nNous vous remercions pour votre confiance et vous souhaitons une excellente journée..\n\nCordialement \n\nEquipe Forma2plus ";

            var smtp = new SmtpClient
            {
                Host = "mail.forma2plus.com",
                Port = 25,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderemail.Address, password)

            };
            using (var mes = new MailMessage(senderemail, receiveremail)
            {
                Subject = subject,
                Body = body,

            })
            {
                smtp.Send(mes);
            }
        }
        public ActionResult Remmeber()
        {
            return View();
        }
            [HttpPost]
        public ActionResult Remmeber(LoginModels model)
        {
          


                List<StagiaireModel> data = StagiaireModel.NumeroDossier(model.mail);
                int numero = 0;
                string nom="";
                string prenom="";
                if (data.Count > 0)
                {
                    StagiaireModel dbUStagiaireObject = data.First();

                    numero = dbUStagiaireObject.Numero;
                  nom = dbUStagiaireObject.nom;
               prenom = dbUStagiaireObject.prenom;
                    //  return RedirectToAction("Plogon", "Home", new { id = model.Numero , datecrea = model.Nom});

                }
                string mail = model.mail;
               


                try
                {
                    SendEmail(mail, numero,nom, prenom);
                    // Rediriger ou afficher un message de succès si l'e-mail est envoyé avec succès
                    ViewBag.ResMessege = ResMessege.getDanger("Votre identifiant a été envoyé à votre adresse e-mail !");
              

                return View();

              
                }
                catch (Exception ex)
                {
                    // Gérer les erreurs d'envoi d'e-mail ici
                    ViewBag.ResMessege = ResMessege.getDanger("Une erreur s'est produite lors de l'envoi de l'e-mail. Veuillez réessayer !");
                    return View();
                }
           

        }
        public ActionResult LockedOut()
        {
            return View();
        }
        [HttpPost]

        public ActionResult Index(LoginModels model)

        {
            if (Session["LoginAttempts"] == null)
            {
                Session["LoginAttempts"] = 0;
            }

            int attempts = (int)Session["LoginAttempts"];

            // Si 2 tentatives échouées, rediriger vers une autre page
            if (attempts >= 2)
            {
                ViewBag.ResMessege = ResMessege.getDanger("Vous avez dépassé le nombre de tentatives de connexion autorisées. Veuillez réessayer plus tard.");
                return View();
                // return RedirectToAction("Notfound"); // Vous pouvez créer cette action et vue
            }

            if (ModelState.IsValid)
            {
               

                List<StagiaireModel> test = StagiaireModel.Email(model.mail);
                if (test == null || test.Count == 0) // Vérifie si la liste est vide
                {
                    ViewBag.ResMessege = ResMessege.getDanger("Veuillez bien renseigner votre adresse mail !");
                    Session["LoginAttempts"] = attempts + 1; // Incrémenter le compteur si l'email est incorrect

                    return View();
                }

               
                    
                    
               
                   
                
                List<StagiaireModel> data = StagiaireModel.Index(model.Numero, model.mail);


                if (data.Count > 0)
                {
                    Session["LoginAttempts"] = 0;

                    Session["userid"] = model.nom + model.Numero;
                    StagiaireModel dbUStagiaireObject = data.First();
                    Session["civilite"] = dbUStagiaireObject.Civilite;
                    Session["fonction"] = dbUStagiaireObject.fonction;
                    Session["telephone"] = dbUStagiaireObject.Telephone;
                    Session["nom"] = dbUStagiaireObject.nom;
                    Session["departement"] = dbUStagiaireObject.Departement;
                    Session["portable"] = dbUStagiaireObject.Portable;
                    Session["prenom"] = dbUStagiaireObject.prenom;
                    Session["numero"] = dbUStagiaireObject.Numero;
                    Session["mail"] = dbUStagiaireObject.mail;
                    Session["societe"] = dbUStagiaireObject.societe;
                    
                    Session["type_mail"] = dbUStagiaireObject.type_mail;

                    string t = TokenManager.GenerateToken(model.Numero);

                    return RedirectToAction("Logins", "Home", new { t });
                  //  return RedirectToAction("Plogon", "Home", new { id = model.Numero , datecrea = model.Nom});

                }
                else
                {
                    ViewBag.ResMessege = ResMessege.getDanger("Ce numéro n'existe pas ou n'a pas été correctement saisi.");
                    Session["LoginAttempts"] = attempts + 1; // Incrémenter le compteur si l'email est incorrect

                    return View();

                }
            }
            else
            {

                return View();

            }

        }
        [HttpGet]

        public ActionResult Plogonn_old(int id)

        {
            // try
            // {
            if (ModelState.IsValid)
            {


                var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, PROFIL.*, personal_interest.*, stagiaire_profil.numero FROM (stagiaire_profil LEFT JOIN personal_interest ON stagiaire_profil.numero = personal_interest.numero_profil) LEFT JOIN PROFIL ON stagiaire_profil.numero = PROFIL.numero WHERE (((stagiaire_profil.numero)=" + id + "));");
                // var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, stagiaire_profil.numero FROM stagiaire_profil WHERE (((stagiaire_profil.numero)="+id+"))");
                if (obj.Count() == 0) { return RedirectToAction("Notfound", "Home"); }
                if (obj != null)
                {
                    StagiaireModel model = new StagiaireModel();


                    model.mail = obj.FirstOrDefault().mail;
                    model.fonction = obj.FirstOrDefault().fonction;
                    model.Numero = obj.FirstOrDefault().Numero;
                    model.NationInterloc = obj.FirstOrDefault().NationInterloc;
                    model.nom = obj.FirstOrDefault().nom;
                    model.Portable = obj.FirstOrDefault().Portable;
                    model.premlang = obj.FirstOrDefault().premlang;
                    model.prenom = obj.FirstOrDefault().prenom;
                    model.Departement = obj.FirstOrDefault().Departement;
                    model.societe = obj.FirstOrDefault().societe;
                    model.Civilite = obj.FirstOrDefault().Civilite;
                    model.numindividu = obj.FirstOrDefault().numindividu;
                    model.type_mail = obj.FirstOrDefault().type_mail;
                    string datas = "&refIndividu=" + model.refindividu + "&mdp=" + model.Numero + "&nom=" + model.nom + "&prenom=" + model.prenom + "";
                    string urls = "http://s371880604.onlinehome.fr/webcalendar_salle/srcs/www/auto.php?module=auto&action=import:updateStagiaire" + "&refIndividu=" + model.numindividu + "&mdp=" + model.Numero + "&nom=" + model.nom + "&prenom=" + model.prenom + "";
                    string StrXMLStagiaire = "<xml><stagiaires><stagiaire>";
                    StrXMLStagiaire = StrXMLStagiaire + "<civilite> " + model.Civilite + "</civilite>";
                    StrXMLStagiaire = StrXMLStagiaire + "<nom>" + model.nom + "</nom>";
                    StrXMLStagiaire = StrXMLStagiaire + "<prenoms>" + model.prenom + "</prenoms>";
                    StrXMLStagiaire = StrXMLStagiaire + "<mail>" + model.mail + "</mail>";
                    StrXMLStagiaire = StrXMLStagiaire + "<fonction>" + model.fonction + "</fonction>";
                    StrXMLStagiaire = StrXMLStagiaire + "<societe>" + model.societe + "</societe>";
                    StrXMLStagiaire = StrXMLStagiaire + "<num_individu>" + model.numindividu + "</num_individu>";
                    StrXMLStagiaire = StrXMLStagiaire + "<telephone>" + model.Telephone + "</telephone>";
                    StrXMLStagiaire = StrXMLStagiaire + "<portable>" + model.Portable + "</portable>";
                    StrXMLStagiaire = StrXMLStagiaire + "<rue_stage>" + model.RueDuStage + "</rue_stage>";
                    StrXMLStagiaire = StrXMLStagiaire + "<ville_stage>" + model.VilleDuStage + "</ville_stage>";
                    StrXMLStagiaire = StrXMLStagiaire + "<Cp_stage>" + model.CPDuStage + "</Cp_stage>";
                    StrXMLStagiaire = StrXMLStagiaire + "</stagiaire> </stagiaires></xml>";
                    string PostedXMLStagiaire = StrXMLStagiaire;
                    var permission = 0;
                    string urlwebcal = "http://vps339112.ovh.net/webcalendar-1.6/www/auto.php/webservices/security/authenticate?";
                    string token = GetTokenFromUrl(model.Numero);
                    string urlWithToken = urlwebcal + "token=" + token;

                    ViewBag.UrlsWithToken = urlWithToken;
                    if (model.type_mail != "P0")
                    {
                        if (model.type_mail == "01")
                        {
                            ViewBag.url = urls;

                        }
                    }
                    List<StagiaireModel> model4 = StagiaireModel.PermssionTel(model.Numero);
                    permission = model4.Count;
                    List<StagiaireModel> model5 = StagiaireModel.PermssionFonction(model.Numero);
                    permission = model5.Count;
                    List<StagiaireModel> model2 = StagiaireModel.Profil(model.Numero);
                    if (permission > 0) { permission = model2.Count; }
                    List<StagiaireModel> model3 = StagiaireModel.Permissions(model.Numero);
                    if (permission > 0) { permission = model3.Count; }
                    ViewBag.permision = permission;
                    List<StagiaireModel> model1 = StagiaireModel.Noteglobal(model.Numero);
                    if (model1.Count > 0)
                    {
                        StagiaireModel dbUStagiaireObject = model1.First();
                    }
                    ViewBag.Note_globale = model1.Count;
                    return View(model);
                }
                else
                {
                    ViewBag.ResMessege = ResMessege.getDanger("Cette numero n'exite pas");
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
        private string GetTextFromUrl(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }

                return null;
            }
        }
        private async Task<string> SendXmlDataAndGetJsonResponseNew(string apiUrl, string xmlData)
        {
            using (HttpClient client = new HttpClient())
            {
                // Construct the URL with parameters
                string fullUrl = $"{apiUrl}?param1=value1&param2=value2"; // Add your parameters here

                // Assuming your API endpoint expects Content-Type as application/xml
                client.DefaultRequestHeaders.Add("Content-Type", "application/xml");

                // Send the XML data as a POST request
                HttpResponseMessage response = await client.PostAsync(fullUrl, new StringContent(xmlData, Encoding.UTF8, "application/xml"));

                if (response.IsSuccessStatusCode)
                {
                    // Read and return the JSON response
                    return await response.Content.ReadAsStringAsync();
                }

                // Handle non-success status code if needed
                return null;
            }
        }
    
    private string GetXmlStringFromAdd(StagiaireModel model)
        {
            XElement stagiaireElement = new XElement("utilisateur",
                new XElement("utilisateur_id", model.numindividu),
                new XElement("utilisateur_iCivilite", model.Civilite),
                new XElement("utilisateur_zNom", model.nom),

                new XElement("utilisateur_zPrenom", model.prenom),
                new XElement("utilisateur_zMail", model.mail),
                new XElement("utilisateur_zTel", model.fonction),
                new XElement("societe", model.societe),

                new XElement("utilisateur_zRefindividu", model.numindividu),
             
                new XElement("utilisateur_Adresse1", model.RueDuStage)
              
            );

            XElement stagiairesElement = new XElement("utilisateur", stagiaireElement);

            return stagiairesElement.ToString();
        }
        private string GetXmlStringFromModel(StagiaireModel model)
        {
            var telph1 = model.Portable;
            var telph2 = model.Telephone;

            if (string.IsNullOrEmpty(telph2) || telph2 == ".")
            {
                telph2 = telph1;
            }

            XElement stagiaireElement = new XElement("stagiaire",
                new XElement("civilite", model.Civilite),
                new XElement("nom", model.nom),
                new XElement("prenoms", model.prenom),
                new XElement("mail", model.mail),
                new XElement("fonction", model.fonction),
                new XElement("societe", model.societe),
                
                new XElement("num_individu", model.Numero),
                new XElement("telephone", telph2),
                new XElement("portable", model.Portable),
                new XElement("rue_stage", model.RueDuStage),
                new XElement("ville_stage", model.VilleDuStage),
                new XElement("Cp_stage", model.CPDuStage)
            );

            XElement stagiairesElement = new XElement("stagiaires", stagiaireElement);

            return stagiairesElement.ToString();
        }

        private string GetAutoPlannificationLink()
        {
            // Implement your logic to get the auto plannification link
            return "http://s371880604.onlinehome.fr/";
        }
        private string GetAutoAddWebcaLink()
        {
            // Implement your logic to get the auto plannification link
            return "http://vps676482.ovh.net/webcalendar-1.6/www/ws.php/ws/import/";
        }
        public string DebutWebcal(string strXMLStagiaire)
        {
            string url = $"{GetAutoPlannificationLink()}/webcalendar/srcs/www/index.php?module=auto&action=import:index&PostedXMLStagiaire={strXMLStagiaire}";
            string sResult = GetTextFromUrl(url);

            ViewBag.WebcalResult = sResult;

            return sResult;
        }

        public string SalleWebcal(int? num, int v_num, string v_nom, string v_prenom)
        {
            string url = $"{GetAutoPlannificationLink()}/webcalendar_salle/srcs/www/auto.php?module=auto&action=import:updateStagiaire&refIndividu={v_num}&mdp={num}&nom={v_nom}&prenom={v_prenom}";
            string URLMaj = GetTextFromUrl(url);
          
            ViewBag.WebcalSalleResult = URLMaj;

            return URLMaj;
        }

       public string ImportNew(string strXLM)
        {

            
            string url = "http://vps676482.ovh.net/webcalendar-1.6/www/ws.php/ws/import/addUserIndividus?";
            url = $"{url}{strXLM}";
            string UrlAdd = GetTextFromUrl(url);
            ViewBag.WebcalImport = UrlAdd;
            return UrlAdd;
        }
        public string ImportNewAdd(string strXLM)
        {

            string url = "http://vps676482.ovh.net/webcalendar-1.6/www/ws.php/ws/import/addStagiaire?";
            url = $"{url}{strXLM}";
            string UrlAdd = GetTextFromUrl(url);
            ViewBag.WebcalImport = UrlAdd;
            return UrlAdd;
        }
        private string GetTokenFromUrl(int? stagiaireID)
        {

            string baseUrl ="https://formateur.forma2plus.com/auto.php/webservices/security/getToken?";
            string targetUrl = "/auto/disponibility/";

            string tokenUrl = $"{baseUrl}stagiaireID={stagiaireID}&target={targetUrl}";// Construire dynamiquement l'URL avec le stagiaireID

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(tokenUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    // Lire le contenu JSON de la réponse
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;

                    // Désérialiser la réponse JSON pour obtenir le token
                    JObject json = JObject.Parse(jsonResponse);
                    bool status = (bool)json["status"];

                    if (status)
                    {
                        // Extraire le token du JSON
                        //string token = (string)json["token"];
                        string token = (string)json["token"];
                        return token;
                    }
                    else
                    {
                        string token = "";
                        return token;
                    }
                }
                else
                {
                    // Gérer les erreurs de la requête HTTP si nécessaire
                    throw new Exception("Erreur lors de la récupération du token");
                }
            }
        }

       
        public class TokenResponse
        {
            public bool Status { get; set; }
            public string Token { get; set; }
        }
        [HttpGet]

        public ActionResult Plogon_ancien(int id)

        {
            // try
            // {
            if (ModelState.IsValid)
            {


                var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, PROFIL.*, personal_interest.*, stagiaire_profil.numero FROM (stagiaire_profil LEFT JOIN personal_interest ON stagiaire_profil.numero = personal_interest.numero_profil) LEFT JOIN PROFIL ON stagiaire_profil.numero = PROFIL.numero WHERE (((stagiaire_profil.numero)=" + id + "));");
                // var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, stagiaire_profil.numero FROM stagiaire_profil WHERE (((stagiaire_profil.numero)="+id+"))");
                if (obj.Count() == 0) { return RedirectToAction("Notfound", "Home"); }
                if (obj != null)
                {
                    StagiaireModel model = new StagiaireModel();


                    model.mail = obj.FirstOrDefault().mail;
                    model.fonction = obj.FirstOrDefault().fonction;
                    model.Numero = obj.FirstOrDefault().Numero;
                    model.NationInterloc = obj.FirstOrDefault().NationInterloc;
                    model.nom = obj.FirstOrDefault().nom;
                    model.Portable = obj.FirstOrDefault().Portable;
                    model.premlang = obj.FirstOrDefault().premlang;
                    model.prenom = obj.FirstOrDefault().prenom;
                    model.Departement = obj.FirstOrDefault().Departement;
                    model.societe = obj.FirstOrDefault().societe;
                    model.Civilite = obj.FirstOrDefault().Civilite;
                    model.numindividu = obj.FirstOrDefault().numindividu;
                    model.type_mail = obj.FirstOrDefault().type_mail;

                    // var obje = postXMLData( UrlData,Postxml);

                    // return RedirectToAction("Intro", "Home");
                    // string TheUrl = "http://s371880604.onlinehome.fr/webcalendar_salle/srcs/www/auto.php?module=auto&action=import:updateStagiaire";
                    string datas = "&refIndividu=" + model.refindividu + "&mdp=" + model.Numero + "&nom=" + model.nom + "&prenom=" + model.prenom + "";
                    string urls = "http://s371880604.onlinehome.fr/webcalendar_salle/srcs/www/auto.php?module=auto&action=import:updateStagiaire" + "&refIndividu=" + model.numindividu + "&mdp=" + model.Numero + "&nom=" + model.nom + "&prenom=" + model.prenom + "";


                    string StrXMLStagiaire = "<xml><stagiaires><stagiaire>";
                    StrXMLStagiaire = StrXMLStagiaire + "<civilite> " + model.Civilite + "</civilite>";
                    StrXMLStagiaire = StrXMLStagiaire + "<nom>" + model.nom + "</nom>";
                    StrXMLStagiaire = StrXMLStagiaire + "<prenoms>" + model.prenom + "</prenoms>";
                    StrXMLStagiaire = StrXMLStagiaire + "<mail>" + model.mail + "</mail>";
                    StrXMLStagiaire = StrXMLStagiaire + "<fonction>" + model.fonction + "</fonction>";
                    StrXMLStagiaire = StrXMLStagiaire + "<societe>" + model.societe + "</societe>";
                    StrXMLStagiaire = StrXMLStagiaire + "<num_individu>" + model.numindividu + "</num_individu>";
                    StrXMLStagiaire = StrXMLStagiaire + "<telephone>" + model.Telephone + "</telephone>";
                    StrXMLStagiaire = StrXMLStagiaire + "<portable>" + model.Portable + "</portable>";
                    StrXMLStagiaire = StrXMLStagiaire + "<rue_stage>" + model.RueDuStage + "</rue_stage>";
                    StrXMLStagiaire = StrXMLStagiaire + "<ville_stage>" + model.VilleDuStage + "</ville_stage>";
                    StrXMLStagiaire = StrXMLStagiaire + "<Cp_stage>" + model.CPDuStage + "</Cp_stage>";

                    StrXMLStagiaire = StrXMLStagiaire + "</stagiaire> </stagiaires></xml>";
                    //string StrXMLStagiaire = "<xml><stagiaires><stagiaire>";
                    //  StrXMLStagiaire =  "civilite=" + model.Civilite + "&";
                    //StrXMLStagiaire = StrXMLStagiaire + "nom=" + model.Nom + "&";
                    // StrXMLStagiaire = StrXMLStagiaire + "prenoms=" + model.Prenom + "&";
                    //  StrXMLStagiaire = StrXMLStagiaire + "mail=" + model.Mail + "&";
                    //  StrXMLStagiaire = StrXMLStagiaire + "fonction=" + model.Fonction + "&";
                    //  StrXMLStagiaire = StrXMLStagiaire + "societe=" + model.Societe + "&";
                    //  StrXMLStagiaire = StrXMLStagiaire + "num_individu=" + model.numindividu + "&";
                    //  StrXMLStagiaire = StrXMLStagiaire + "telephone=" + model.Telephone +"&";
                    //  StrXMLStagiaire = StrXMLStagiaire + "portable=" + model.Portable +"&";
                    //  StrXMLStagiaire = StrXMLStagiaire + "rue_stage=" + model.RueDuStage +"&";
                    //  StrXMLStagiaire = StrXMLStagiaire + "ville_stage="+ model.VilleDuStage;
                    // StrXMLStagiaire = StrXMLStagiaire + "Cp_stage=" + model.CPDuStage + "&";

                    //StrXMLStagiaire = StrXMLStagiaire + "</stagiaire> </stagiaires></xml>";
                    // string urlpaht = "http://s371880604.onlinehome.fr/webcalendar/srcs/www/index.php?module=auto&action=import:index&PostedXMLStagiaire=";

                    //  string urly = "http://s371880604.onlinehome.fr/webcalendar/srcs/www/index.php?module=auto&action=import:index";
                    // Httpcontent PostedXMLStagiaire = new HttpContent();
                    string PostedXMLStagiaire = StrXMLStagiaire;
                    // var envoie = InsertIp(model.numindividu);
                    var permission = 0;
                    if (model.type_mail != "P0")
                    {
                        if (model.type_mail == "01")
                        {
                            // var response = GetPHP(urls);
                            //var reponsesPlanifier = getProductLIstXML(urlpaht,PostedXMLStagiaire);

                            //var reponsesPlanifier = GetPHPXML(urly, StrXMLStagiaire);
                            ViewBag.url = urls;
                            //ViewBag.response = response;
                            //ViewBag.urlplan = urlpaht;
                            // ViewBag.responsesPlanifier = reponsesPlanifier;


                            //  sendEmailConfirmationButErreur(email, ref_we, ref_order, amount, ex.Message);
                            // return ViewBag.reponsesPlanifier;

                        }
                        /*  if (model.type_mail == "P1")
                          {
                             // var response = GetPHP(urls);
                              var reponsesPlanifier = GetPHP(urlpaht);
                             // ViewBag.url = urls;
                             // ViewBag.response = response;
                              ViewBag.urlplan = urlpaht;
                              ViewBag.responsesPlanifier = reponsesPlanifier;
                          }*/

                    }
                    List<StagiaireModel> model4 = StagiaireModel.PermssionTel(model.Numero);

                    permission = model4.Count;
                    List<StagiaireModel> model5 = StagiaireModel.PermssionFonction(model.Numero);

                    permission = model5.Count;

                    List<StagiaireModel> model2 = StagiaireModel.Profil(model.Numero);

                    //var obj23= cnn.Execute(@"SELECT test_en_ligne.numero_idbase, test_en_ligne.* FROM test_en_ligne WHERE (((test_en_ligne.numero_idbase)=" + id + "))");

                    if (permission > 0) { permission = model2.Count; }



                    List<StagiaireModel> model3 = StagiaireModel.Permissions(model.Numero);

                    if (permission > 0) { permission = model3.Count; }
                    ViewBag.permision = permission;
                    List<StagiaireModel> model1 = StagiaireModel.Noteglobal(model.Numero);

                    //var obj2 = cnn.Execute(@"SELECT test_en_ligne.numero_idbase, test_en_ligne.* FROM test_en_ligne WHERE (((test_en_ligne.numero_idbase)=" + id + "))");
                    if (model1.Count > 0)
                    {
                        StagiaireModel dbUStagiaireObject = model1.First();
                    }
                    ViewBag.Note_globale = model1.Count;


                    return View(model);


                }
                else
                {
                    ViewBag.ResMessege = ResMessege.getDanger("Cette numero n'exite pas");
                    return View();

                }

            }
            else
            {

                return View();

            }
            //   }
            //  catch (Exception ex)
            //  {
            // return RedirectToAction("Notfound", "Home");

            //}
        }

        [HttpGet]

        public ActionResult PlogonTest(string num)

        {
            // try
            // {
            if (ModelState.IsValid)
            {

             //   int id = UrlEncryptorTest.DecryptOK(num);
                   var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, PROFIL.*, personal_interest.*, stagiaire_profil.numero FROM (stagiaire_profil LEFT JOIN personal_interest ON stagiaire_profil.numero = personal_interest.numero_profil) LEFT JOIN PROFIL ON stagiaire_profil.numero = PROFIL.numero WHERE (((stagiaire_profil.numero)=" + num + "));");
                   //var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, stagiaire_profil.numero FROM stagiaire_profil WHERE (((stagiaire_profil.numero)="+id+"))");
                    if (obj.Count() == 0) { return RedirectToAction("Notfound", "Home"); }
                    if (obj != null)
                    {
                        StagiaireModel model = new StagiaireModel();


                        model.mail = obj.FirstOrDefault().mail;
                        model.fonction = obj.FirstOrDefault().fonction;
                        model.Numero = obj.FirstOrDefault().Numero;
                        model.NationInterloc = obj.FirstOrDefault().NationInterloc;
                        model.nom = obj.FirstOrDefault().nom;
                        model.Portable = obj.FirstOrDefault().Portable;
                        model.premlang = obj.FirstOrDefault().premlang;
                        model.prenom = obj.FirstOrDefault().prenom;
                        model.Departement = obj.FirstOrDefault().Departement;
                        model.societe = obj.FirstOrDefault().societe;
                        model.Civilite = obj.FirstOrDefault().Civilite;
                        model.numindividu = obj.FirstOrDefault().numindividu;
                        model.type_mail = obj.FirstOrDefault().type_mail;

                        // var obje = postXMLData( UrlData,Postxml);

                        // return RedirectToAction("Intro", "Home");
                        // string TheUrl = "http://s371880604.onlinehome.fr/webcalendar_salle/srcs/www/auto.php?module=auto&action=import:updateStagiaire";
                        string datas = "&refIndividu=" + model.refindividu + "&mdp=" + model.Numero + "&nom=" + model.nom + "&prenom=" + model.prenom + "";
                        string urls = "http://s371880604.onlinehome.fr/webcalendar_salle/srcs/www/auto.php?module=auto&action=import:updateStagiaire" + "&refIndividu=" + model.numindividu + "&mdp=" + model.Numero + "&nom=" + model.nom + "&prenom=" + model.prenom + "";


                       
                        string urlwebcal = "https://formateur.forma2plus.com/auto.php/webservices/security/authenticate?";
                        string token = GetTokenFromUrl(model.numindividu); // Supposons que cette fonction retourne le jeton
                        string urlWithToken = $"{urlwebcal}token={token}";
                        ViewBag.urlwebcal = urlWithToken;
                        ViewBag.idInd = model.numindividu;

                        // var envoie = InsertIp(model.numindividu);
                        var permission = 0;
                        if (model.type_mail != "P0")
                        {
                            if (model.type_mail == "01")
                            {
                                // var response = GetPHP(urls);
                                //var reponsesPlanifier = getProductLIstXML(urlpaht,PostedXMLStagiaire);

                                //var reponsesPlanifier = GetPHPXML(urly, StrXMLStagiaire);
                                ViewBag.url = urls;
                                //ViewBag.response = response;
                                //ViewBag.urlplan = urlpaht;
                                // ViewBag.responsesPlanifier = reponsesPlanifier;


                                //  sendEmailConfirmationButErreur(email, ref_we, ref_order, amount, ex.Message);
                                // return ViewBag.reponsesPlanifier;

                            }
                            /*  if (model.type_mail == "P1")
                              {
                                 // var response = GetPHP(urls);
                                  var reponsesPlanifier = GetPHP(urlpaht);
                                 // ViewBag.url = urls;
                                 // ViewBag.response = response;
                                  ViewBag.urlplan = urlpaht;
                                  ViewBag.responsesPlanifier = reponsesPlanifier;
                              }*/

                        }
                        List<StagiaireModel> model4 = StagiaireModel.PermssionTel(model.Numero);

                        permission = model4.Count;
                        List<StagiaireModel> model5 = StagiaireModel.PermssionFonction(model.Numero);

                        permission = model5.Count;

                        List<StagiaireModel> model2 = StagiaireModel.Profil(model.Numero);

                        //var obj23= cnn.Execute(@"SELECT test_en_ligne.numero_idbase, test_en_ligne.* FROM test_en_ligne WHERE (((test_en_ligne.numero_idbase)=" + id + "))");

                        if (permission > 0) { permission = model2.Count; }



                        List<StagiaireModel> model3 = StagiaireModel.Permissions(model.Numero);

                        if (permission > 0) { permission = model3.Count; }
                        ViewBag.permision = permission;
                        List<StagiaireModel> model1 = StagiaireModel.Noteglobal(model.Numero);

                        //var obj2 = cnn.Execute(@"SELECT test_en_ligne.numero_idbase, test_en_ligne.* FROM test_en_ligne WHERE (((test_en_ligne.numero_idbase)=" + id + "))");
                        if (model1.Count > 0)
                        {
                            StagiaireModel dbUStagiaireObject = model1.First();
                        }
                        ViewBag.Note_globale = model1.Count;


                        return View(model);


                    }

                    else
                    {
                        ViewBag.ResMessege = ResMessege.getDanger("Cette numero n'exite pas");
                        return View();

                    }

                }
                else
                {
                    return RedirectToAction("Index", "Home");

                }
           

        }

        [HttpGet]

        public ActionResult Plogon(int id, string datecrea)

        {
            // try
            // {
            if (ModelState.IsValid)
            {

                //   int id = UrlEncryptorTest.DecryptOK(num);
                // var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, PROFIL.*, personal_interest.*, stagiaire_profil.numero FROM (stagiaire_profil LEFT JOIN personal_interest ON stagiaire_profil.numero = personal_interest.numero_profil) LEFT JOIN PROFIL ON stagiaire_profil.numero = PROFIL.numero WHERE (((stagiaire_profil.numero)=" + num + "));");
                //var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, stagiaire_profil.numero FROM stagiaire_profil WHERE (((stagiaire_profil.numero)="+id+"))");
                var Test = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, stagiaire_profil.numero FROM stagiaire_profil WHERE stagiaire_profil.numero=" + id + ";");
                if (Test.Count() == 0) { return RedirectToAction("Index", "Home"); }
                var compdate = "";
                if (Test != null)
                {
                    StagiaireModel models = new StagiaireModel();
                    models.nom = Test.FirstOrDefault().nom;
                    models.Numero = Test.FirstOrDefault().Numero;
                    compdate = models.Date_creation.Day.ToString();
                    compdate = models.nom;
                    compdate = compdate.Substring(0, 2); // Récupérer les deux premières lettres

                }
                if (datecrea != null)
                {
                    datecrea = datecrea.Substring(0, 2); // Récupérer les deux premières lettres
                    datecrea = datecrea.ToUpper();
                }

                compdate = compdate.ToUpper();
                if (compdate == datecrea)
                {

                    var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, PROFIL.*, personal_interest.*, stagiaire_profil.numero FROM (stagiaire_profil LEFT JOIN personal_interest ON stagiaire_profil.numero = personal_interest.numero_profil) LEFT JOIN PROFIL ON stagiaire_profil.numero = PROFIL.numero WHERE (((stagiaire_profil.numero)=" + id + "));");

                    if (obj.Count() == 0) { return RedirectToAction("Notfound", "Home"); }
                    if (obj != null)
                    {
                        StagiaireModel model = new StagiaireModel();


                        model.mail = obj.FirstOrDefault().mail;
                        model.fonction = obj.FirstOrDefault().fonction;
                        model.Numero = obj.FirstOrDefault().Numero;
                        model.NationInterloc = obj.FirstOrDefault().NationInterloc;
                        model.nom = obj.FirstOrDefault().nom;
                        model.Portable = obj.FirstOrDefault().Portable;
                        model.premlang = obj.FirstOrDefault().premlang;
                        model.prenom = obj.FirstOrDefault().prenom;
                        model.Departement = obj.FirstOrDefault().Departement;
                        model.societe = obj.FirstOrDefault().societe;
                        model.Civilite = obj.FirstOrDefault().Civilite;
                        model.numindividu = obj.FirstOrDefault().numindividu;
                        model.type_mail = obj.FirstOrDefault().type_mail;

                        // var obje = postXMLData( UrlData,Postxml);

                        // return RedirectToAction("Intro", "Home");
                        // string TheUrl = "http://s371880604.onlinehome.fr/webcalendar_salle/srcs/www/auto.php?module=auto&action=import:updateStagiaire";
                        string datas = "&refIndividu=" + model.refindividu + "&mdp=" + model.Numero + "&nom=" + model.nom + "&prenom=" + model.prenom + "";
                        string urls = "http://s371880604.onlinehome.fr/webcalendar_salle/srcs/www/auto.php?module=auto&action=import:updateStagiaire" + "&refIndividu=" + model.numindividu + "&mdp=" + model.Numero + "&nom=" + model.nom + "&prenom=" + model.prenom + "";






                        // var envoie = InsertIp(model.numindividu);
                        var permission = 0;
                        if (model.type_mail != "P0")
                        {
                            if (model.type_mail == "01")
                            {

                                // var response = GetPHP(urls);
                                //var reponsesPlanifier = getProductLIstXML(urlpaht,PostedXMLStagiaire);

                                //var reponsesPlanifier = GetPHPXML(urly, StrXMLStagiaire);
                                ViewBag.url = urls;
                                //ViewBag.response = response;
                                //ViewBag.urlplan = urlpaht;
                                // ViewBag.responsesPlanifier = reponsesPlanifier;


                                //  sendEmailConfirmationButErreur(email, ref_we, ref_order, amount, ex.Message);
                                // return ViewBag.reponsesPlanifier;

                            }
                            /*  if (model.type_mail == "P1")
                              {
                                 // var response = GetPHP(urls);
                                  var reponsesPlanifier = GetPHP(urlpaht);
                                 // ViewBag.url = urls;
                                 // ViewBag.response = response;
                                  ViewBag.urlplan = urlpaht;
                                  ViewBag.responsesPlanifier = reponsesPlanifier;
                              }*/

                        }
                        List<StagiaireModel> model4 = StagiaireModel.PermssionTel(model.Numero);

                        permission = model4.Count;
                        /* List<StagiaireModel> model5 = StagiaireModel.PermssionFonction(model.Numero);

                         permission = model5.Count;*/

                        List<StagiaireModel> model2 = StagiaireModel.Profil(model.Numero);
                        if (model2.Count > 0)
                        {
                            string urlwebcal = "https://formateur.forma2plus.com/auto.php/webservices/security/authenticate?";
                            string token = GetTokenFromUrl(model.numindividu); // Supposons que cette fonction retourne le jeton
                            string urlWithToken = $"{urlwebcal}token={token}";
                            ViewBag.urlwebcal = urlWithToken;
                            ViewBag.idInd = model.numindividu;
                        }
                        //var obj23= cnn.Execute(@"SELECT test_en_ligne.numero_idbase, test_en_ligne.* FROM test_en_ligne WHERE (((test_en_ligne.numero_idbase)=" + id + "))");

                        if (permission > 0) { permission = model2.Count; }



                        List<StagiaireModel> model3 = StagiaireModel.Permissions(model.Numero);

                        if (permission > 0) { permission = model3.Count; }
                        ViewBag.permision = permission;
                        List<StagiaireModel> model1 = StagiaireModel.Noteglobal(model.Numero);

                        //var obj2 = cnn.Execute(@"SELECT test_en_ligne.numero_idbase, test_en_ligne.* FROM test_en_ligne WHERE (((test_en_ligne.numero_idbase)=" + id + "))");
                        if (model1.Count > 0)
                        {
                            StagiaireModel dbUStagiaireObject = model1.First();
                        }
                        ViewBag.Note_globale = model1.Count;


                        return View(model);


                    }

                    else
                    {
                        ViewBag.ResMessege = ResMessege.getDanger("Cette numero n'exite pas");
                        return View();

                    }

                }
                else
                {
                    return RedirectToAction("Index", "Home");

                }
            }
            else
            {

                return View();

            }


        }

        [HttpGet]

        public ActionResult Logins(string t)

        {
            // try
            // {
            if (ModelState.IsValid)
            {
                int id = TokenManager.ValidateToken(t);


                //   int id = UrlEncryptorTest.DecryptOK(num);
                // var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, PROFIL.*, personal_interest.*, stagiaire_profil.numero FROM (stagiaire_profil LEFT JOIN personal_interest ON stagiaire_profil.numero = personal_interest.numero_profil) LEFT JOIN PROFIL ON stagiaire_profil.numero = PROFIL.numero WHERE (((stagiaire_profil.numero)=" + num + "));");
                //var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, stagiaire_profil.numero FROM stagiaire_profil WHERE (((stagiaire_profil.numero)="+id+"))");
                var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, stagiaire_profil.numero FROM stagiaire_profil WHERE stagiaire_profil.numero=" + id + ";");
               // if (Test.Count() == 0) { return RedirectToAction("Notfound", "Home"); }
                
                //    var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, PROFIL.*, personal_interest.*, stagiaire_profil.numero FROM (stagiaire_profil LEFT JOIN personal_interest ON stagiaire_profil.numero = personal_interest.numero_profil) LEFT JOIN PROFIL ON stagiaire_profil.numero = PROFIL.numero WHERE (((stagiaire_profil.numero)=" + id + "));");

                    if (obj.Count() == 0) { return RedirectToAction("Index", "Home"); }
                HttpCookie consentCookie = Request.Cookies["CookieConsent"];
                if (consentCookie != null && consentCookie.Value == "Accepted")
                {
                    // L'utilisateur a accepté les cookies
                    ViewBag.CookieConsent = true;
                }
                else
                {
                    // L'utilisateur n'a pas encore accepté les cookies
                    ViewBag.CookieConsent = false;
                }

                if (obj != null)
                    {
                        StagiaireModel model = new StagiaireModel();


                        model.mail = obj.FirstOrDefault().mail;
                        model.fonction = obj.FirstOrDefault().fonction;
                        model.Numero = obj.FirstOrDefault().Numero;
                        model.NationInterloc = obj.FirstOrDefault().NationInterloc;
                        model.nom = obj.FirstOrDefault().nom;
                        model.Portable = obj.FirstOrDefault().Portable;
                        model.premlang = obj.FirstOrDefault().premlang;
                        model.prenom = obj.FirstOrDefault().prenom;
                        model.Departement = obj.FirstOrDefault().Departement;
                        model.societe = obj.FirstOrDefault().societe;
                        model.Civilite = obj.FirstOrDefault().Civilite;
                        model.numindividu = obj.FirstOrDefault().numindividu;
                        model.type_mail = obj.FirstOrDefault().type_mail;

                        // var obje = postXMLData( UrlData,Postxml);

                        // return RedirectToAction("Intro", "Home");
                        // string TheUrl = "http://s371880604.onlinehome.fr/webcalendar_salle/srcs/www/auto.php?module=auto&action=import:updateStagiaire";
                        string datas = "&refIndividu=" + model.refindividu + "&mdp=" + model.Numero + "&nom=" + model.nom + "&prenom=" + model.prenom + "";
                        string urls = "http://s371880604.onlinehome.fr/webcalendar_salle/srcs/www/auto.php?module=auto&action=import:updateStagiaire" + "&refIndividu=" + model.numindividu + "&mdp=" + model.Numero + "&nom=" + model.nom + "&prenom=" + model.prenom + "";


                        
                        
                        

                        // var envoie = InsertIp(model.numindividu);
                        var permission = 0;
                        if (model.type_mail != "P0")
                        {
                            if (model.type_mail == "01")
                            {

                             
                                ViewBag.url = urls;
                               

                            }
                          

                        }
                        List<StagiaireModel> model4 = StagiaireModel.PermssionTel(model.Numero);

                       permission = model4.Count;
                       /* List<StagiaireModel> model5 = StagiaireModel.PermssionFonction(model.Numero);

                        permission = model5.Count;*/

                        List<StagiaireModel> model2 = StagiaireModel.Profil(model.Numero);
                        if (model2.Count>0)
                        {
                            string urlwebcal = "https://formateur.forma2plus.com/auto.php/webservices/security/authenticate?";
                            string token = GetTokenFromUrl(model.numindividu); // Supposons que cette fonction retourne le jeton
                            string urlWithToken = $"{urlwebcal}token={token}";
                            ViewBag.urlwebcal = urlWithToken;
                            ViewBag.idInd = model.numindividu;
                        }
                        //var obj23= cnn.Execute(@"SELECT test_en_ligne.numero_idbase, test_en_ligne.* FROM test_en_ligne WHERE (((test_en_ligne.numero_idbase)=" + id + "))");

                        if (permission > 0) { permission = model2.Count; }



                        List<StagiaireModel> model3 = StagiaireModel.Permissions(model.Numero);

                        if (permission > 0) { permission = model3.Count; }
                        ViewBag.permision = permission;
                        List<StagiaireModel> model1 = StagiaireModel.Noteglobal(model.Numero);

                        //var obj2 = cnn.Execute(@"SELECT test_en_ligne.numero_idbase, test_en_ligne.* FROM test_en_ligne WHERE (((test_en_ligne.numero_idbase)=" + id + "))");
                        if (model1.Count > 0)
                        {
                            StagiaireModel dbUStagiaireObject = model1.First();
                        }
                        ViewBag.Note_globale = model1.Count;


                        return View(model);

                   
                }

                else
                {
                    ViewBag.ResMessege = ResMessege.getDanger("Cette numero n'exite pas");
                    return View();

                }

           
            }
            else
            {

                return View();

            }


        }

        [HttpPost]
        public ActionResult RememberEmail(string email, bool rememberMe)
        {
            if (rememberMe)
            {
                // Enregistrer l'e-mail dans un cookie
                Response.Cookies["RememberedEmail"].Value = email;
                // Le cookie expire dans 30 jours (par exemple)
                Response.Cookies["RememberedEmail"].Expires = DateTime.Now.AddDays(30);
            }
            else
            {
                // Supprimer le cookie si "se souvenir de moi" n'est pas coché
                Response.Cookies["RememberedEmail"].Expires = DateTime.Now.AddDays(-1);
            }

            // Rediriger vers une autre action, par exemple la page d'accueil
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]

        public ActionResult PlogonErreur(int id, string datecrea)

        {
            // try
            // {
            if (ModelState.IsValid)
            {
                var Test = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, stagiaire_profil.numero FROM stagiaire_profil WHERE stagiaire_profil.numero=" + id + ";");
                if (Test.Count() == 0) { return RedirectToAction("Notfound", "Home"); }
                var compdate = "";
                if (Test != null)
                {
                    StagiaireModel models = new StagiaireModel();
                    models.nom = Test.FirstOrDefault().nom;
                    models.Numero = Test.FirstOrDefault().Numero;
                    compdate = models.Date_creation.Day.ToString();
                    compdate = models.nom;
                    compdate = compdate.Substring(0, 2); // Récupérer les deux premières lettres

                }
                if (datecrea != null)
                {
                    datecrea = datecrea.Substring(0, 2); // Récupérer les deux premières lettres
                    datecrea = datecrea.ToUpper();
                }

                compdate = compdate.ToUpper();
                if (compdate == datecrea)
                {

                    var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, PROFIL.*, personal_interest.*, stagiaire_profil.numero FROM (stagiaire_profil LEFT JOIN personal_interest ON stagiaire_profil.numero = personal_interest.numero_profil) LEFT JOIN PROFIL ON stagiaire_profil.numero = PROFIL.numero WHERE (((stagiaire_profil.numero)=" + id + "));");
                    //var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, stagiaire_profil.numero FROM stagiaire_profil WHERE (((stagiaire_profil.numero)="+id+"))");
                    if (obj.Count() == 0) { return RedirectToAction("Notfound", "Home"); }
                    if (obj != null)
                    {
                        StagiaireModel model = new StagiaireModel();


                        model.mail = obj.FirstOrDefault().mail;
                        model.fonction = obj.FirstOrDefault().fonction;
                        model.Numero = obj.FirstOrDefault().Numero;
                        model.NationInterloc = obj.FirstOrDefault().NationInterloc;
                        model.nom = obj.FirstOrDefault().nom;
                        model.Portable = obj.FirstOrDefault().Portable;
                        model.premlang = obj.FirstOrDefault().premlang;
                        model.prenom = obj.FirstOrDefault().prenom;
                        model.Departement = obj.FirstOrDefault().Departement;
                        model.societe = obj.FirstOrDefault().societe;
                        model.Civilite = obj.FirstOrDefault().Civilite;
                        model.numindividu = obj.FirstOrDefault().numindividu;
                        model.type_mail = obj.FirstOrDefault().type_mail;

                        // var obje = postXMLData( UrlData,Postxml);

                        // return RedirectToAction("Intro", "Home");
                        // string TheUrl = "http://s371880604.onlinehome.fr/webcalendar_salle/srcs/www/auto.php?module=auto&action=import:updateStagiaire";
                        string datas = "&refIndividu=" + model.refindividu + "&mdp=" + model.Numero + "&nom=" + model.nom + "&prenom=" + model.prenom + "";
                        string urls = "http://s371880604.onlinehome.fr/webcalendar_salle/srcs/www/auto.php?module=auto&action=import:updateStagiaire" + "&refIndividu=" + model.numindividu + "&mdp=" + model.Numero + "&nom=" + model.nom + "&prenom=" + model.prenom + "";


                        string StrXMLStagiaire = "<xml><stagiaires><stagiaire>";
                        StrXMLStagiaire = StrXMLStagiaire + "<civilite> " + model.Civilite + "</civilite>";
                        StrXMLStagiaire = StrXMLStagiaire + "<nom>" + model.nom + "</nom>";
                        StrXMLStagiaire = StrXMLStagiaire + "<prenoms>" + model.prenom + "</prenoms>";
                        StrXMLStagiaire = StrXMLStagiaire + "<mail>" + model.mail + "</mail>";
                        StrXMLStagiaire = StrXMLStagiaire + "<fonction>" + model.fonction + "</fonction>";
                        StrXMLStagiaire = StrXMLStagiaire + "<societe>" + model.societe + "</societe>";
                        StrXMLStagiaire = StrXMLStagiaire + "<num_individu>" + model.numindividu + "</num_individu>";
                        StrXMLStagiaire = StrXMLStagiaire + "<telephone>" + model.Telephone + "</telephone>";
                        StrXMLStagiaire = StrXMLStagiaire + "<portable>" + model.Portable + "</portable>";
                        StrXMLStagiaire = StrXMLStagiaire + "<rue_stage>" + model.RueDuStage + "</rue_stage>";
                        StrXMLStagiaire = StrXMLStagiaire + "<ville_stage>" + model.VilleDuStage + "</ville_stage>";
                        StrXMLStagiaire = StrXMLStagiaire + "<Cp_stage>" + model.CPDuStage + "</Cp_stage>";

                        StrXMLStagiaire = StrXMLStagiaire + "</stagiaire> </stagiaires></xml>";
                        //string StrXMLStagiaire = "<xml><stagiaires><stagiaire>";
                        //  StrXMLStagiaire =  "civilite=" + model.Civilite + "&";
                        //StrXMLStagiaire = StrXMLStagiaire + "nom=" + model.Nom + "&";
                        // StrXMLStagiaire = StrXMLStagiaire + "prenoms=" + model.Prenom + "&";
                        //  StrXMLStagiaire = StrXMLStagiaire + "mail=" + model.Mail + "&";
                        //  StrXMLStagiaire = StrXMLStagiaire + "fonction=" + model.Fonction + "&";
                        //  StrXMLStagiaire = StrXMLStagiaire + "societe=" + model.Societe + "&";
                        //  StrXMLStagiaire = StrXMLStagiaire + "num_individu=" + model.numindividu + "&";
                        //  StrXMLStagiaire = StrXMLStagiaire + "telephone=" + model.Telephone +"&";
                        //  StrXMLStagiaire = StrXMLStagiaire + "portable=" + model.Portable +"&";
                        //  StrXMLStagiaire = StrXMLStagiaire + "rue_stage=" + model.RueDuStage +"&";
                        //  StrXMLStagiaire = StrXMLStagiaire + "ville_stage="+ model.VilleDuStage;
                        // StrXMLStagiaire = StrXMLStagiaire + "Cp_stage=" + model.CPDuStage + "&";

                        //StrXMLStagiaire = StrXMLStagiaire + "</stagiaire> </stagiaires></xml>";
                        // string urlpaht = "http://s371880604.onlinehome.fr/webcalendar/srcs/www/index.php?module=auto&action=import:index&PostedXMLStagiaire=";

                        //  string urly = "http://s371880604.onlinehome.fr/webcalendar/srcs/www/index.php?module=auto&action=import:index";
                        // Httpcontent PostedXMLStagiaire = new HttpContent();
                      

                        // var envoie = InsertIp(model.numindividu);
                        var permission = 0;
                        string urlwebcal = "http://vps339112.ovh.net/webcalendar-1.6/www/auto.php/webservices/security/authenticate?";
                        string token = GetTokenFromUrl(model.Numero);
                        string urlWithToken = urlwebcal + "token=" + token;

                        ViewBag.UrlsWithToken = urlWithToken;
                        if (model.type_mail != "P0")
                        {
                            if (model.type_mail == "01")
                            {
                                // var response = GetPHP(urls);
                                //var reponsesPlanifier = getProductLIstXML(urlpaht,PostedXMLStagiaire);

                                //var reponsesPlanifier = GetPHPXML(urly, StrXMLStagiaire);
                                ViewBag.url = urls;
                                //ViewBag.response = response;
                                //ViewBag.urlplan = urlpaht;
                                // ViewBag.responsesPlanifier = reponsesPlanifier;


                                //  sendEmailConfirmationButErreur(email, ref_we, ref_order, amount, ex.Message);
                                // return ViewBag.reponsesPlanifier;

                            }
                        }




                        
                        List<StagiaireModel> model4 = StagiaireModel.PermssionTel(model.Numero);

                        permission = model4.Count;
                        List<StagiaireModel> model5 = StagiaireModel.PermssionFonction(model.Numero);

                        permission = model5.Count;

                        List<StagiaireModel> model2 = StagiaireModel.Profil(model.Numero);

                        //var obj23= cnn.Execute(@"SELECT test_en_ligne.numero_idbase, test_en_ligne.* FROM test_en_ligne WHERE (((test_en_ligne.numero_idbase)=" + id + "))");

                        if (permission > 0) { permission = model2.Count; }



                        List<StagiaireModel> model3 = StagiaireModel.Permissions(model.Numero);

                        if (permission > 0) { permission = model3.Count; }
                        ViewBag.permision = permission;
                        List<StagiaireModel> model1 = StagiaireModel.Noteglobal(model.Numero);

                        //var obj2 = cnn.Execute(@"SELECT test_en_ligne.numero_idbase, test_en_ligne.* FROM test_en_ligne WHERE (((test_en_ligne.numero_idbase)=" + id + "))");
                        if (model1.Count > 0)
                        {
                            StagiaireModel dbUStagiaireObject = model1.First();
                        }
                        ViewBag.Note_globale = model1.Count;
                       
                            

                        return View(model);


                    }

                    else
                    {
                        ViewBag.ResMessege = ResMessege.getDanger("Cette numero n'exite pas");
                        return View();

                    }

                }

                else
                {
                    return RedirectToAction("Index", "Home");

                }
            }
            else
            {

                return View();

            }

        }
        private static string DoGET(string URL, string data)
        {
            byte[] datas = Encoding.ASCII.GetBytes(data);

            var wb = new WebClient();

            wb.BaseAddress = URL;

            //    var response = wb.UploadData(URL, "POST",datas);
            var response = wb.UploadStringTaskAsync(URL, data);

            WebRequest request = WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(datas, 0, datas.Length);
            }

            string responseContent = null;

            using (WebResponse reponse = request.GetResponse())
            {
                using (Stream stream = reponse.GetResponseStream())
                {
                    using (StreamReader sr99 = new StreamReader(stream))
                    {
                        responseContent = sr99.ReadToEnd();
                    }
                }
            }


            return responseContent;
        }
        public ActionResult success()
        {


            return View();
        }

        public ActionResult Notfound()
        {
            return View();
        }
        public ActionResult Edit()
        {
            return View();
        }
        [HttpGet]
        public ActionResult EditprofilSE(int id)
        {
            try
            {

                var Test = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, PROFIL.*, personal_interest.*, stagiaire_profil.numero FROM (stagiaire_profil LEFT JOIN personal_interest ON stagiaire_profil.numero = personal_interest.numero_profil) LEFT JOIN PROFIL ON stagiaire_profil.numero = PROFIL.numero WHERE (((stagiaire_profil.numero)=" + id + "));");
                if (Test.Count() == 0) { return RedirectToAction("Notfound", "Home"); }
                string userid = "";
                if (Test != null)
                {
                    StagiaireModel models = new StagiaireModel();
                    models.nom = Test.FirstOrDefault().nom;
                    models.Numero = Test.FirstOrDefault().Numero;
                    userid= models.nom + models.Numero;
                }
                   
                   
                    // var obj = cnn.Query<StagiaireModel>(@"select S.numero,numindividu,civilite,fonction,telephone,nom,prenom,departement,portable,mail,societe,joursouhait,tempsprof,RueDuStage,VilleDuStage,CPDuStage,accesnetpro,horaisouhait,e_learning,accesnetperso,nbabscprevue,P.numero,niveausco,languemat,nbansbac,premlang,paysanglo,LangGen,LangPro,NationInterloc,FonctInterloc,BesoinsSpecif,AttentesSpec,formatAnt,AttentesGramm,AttentesCompreh,AttentesVocab,AttentesSpec,ConfrAccueilVisite,ConfrLecture,ConfrOral,ConfrPresent,ConfrNegoc,ObjStage,sport,jardin,musique,theatre,arts,sciences,litterature,bricolage,cuisine,autres_interets  from STAGIAIRE S right JOIN PROFIL P ON S.numero =P.numero where (S.numero=" + id + ");");
                    var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, PROFIL.*, personal_interest.*, stagiaire_profil.numero FROM (stagiaire_profil LEFT JOIN personal_interest ON stagiaire_profil.numero = personal_interest.numero_profil) LEFT JOIN PROFIL ON stagiaire_profil.numero = PROFIL.numero WHERE (((stagiaire_profil.numero)=" + id + "));");
                    if (obj.Count() == 0) { return RedirectToAction("Notfound", "Home"); }

                    if (obj != null)
                    {
                        StagiaireModel model = new StagiaireModel();

                        model.mail = obj.FirstOrDefault().mail;
                        model.fonction = WebUtility.HtmlDecode(obj.FirstOrDefault().fonction);
                        model.numindividu = obj.FirstOrDefault().numindividu;
                        model.Numero = obj.FirstOrDefault().Numero;
                        model.NationInterloc = obj.FirstOrDefault().NationInterloc;
                        model.nom = WebUtility.HtmlDecode(obj.FirstOrDefault().nom);
                        model.Portable = obj.FirstOrDefault().Portable;
                        model.premlang = WebUtility.HtmlDecode(obj.FirstOrDefault().premlang);
                        model.prenom = WebUtility.HtmlDecode(obj.FirstOrDefault().prenom);
                        model.Departement = WebUtility.HtmlDecode(obj.FirstOrDefault().Departement);
                        model.societe = WebUtility.HtmlDecode(obj.FirstOrDefault().societe);
                        model.Civilite = obj.FirstOrDefault().Civilite;
                        model.CPDuStage = WebUtility.HtmlDecode(obj.FirstOrDefault().CPDuStage);
                        model.tempsperso = obj.FirstOrDefault().tempsperso;
                        model.tempsprof = obj.FirstOrDefault().tempsprof;
                        model.RueDuStage = WebUtility.HtmlDecode(obj.FirstOrDefault().RueDuStage);
                        model.Telephone = obj.FirstOrDefault().Telephone;
                        model.joursouhait = WebUtility.HtmlDecode(obj.FirstOrDefault().joursouhait);
                        model.accesnetperso = obj.FirstOrDefault().accesnetperso;
                        model.accesnetpro = obj.FirstOrDefault().accesnetpro;
                        model.AttentesSpec = WebUtility.HtmlDecode(obj.FirstOrDefault().AttentesSpec);
                        model.E_learning = obj.FirstOrDefault().E_learning;
                        model.niveausco = obj.FirstOrDefault().niveausco;
                        model.VilleDuStage = WebUtility.HtmlDecode(obj.FirstOrDefault().VilleDuStage);
                        model.FonctInterloc = WebUtility.HtmlDecode(obj.FirstOrDefault().FonctInterloc);
                        model.paysanglo = WebUtility.HtmlDecode(obj.FirstOrDefault().paysanglo);
                        model.languemat = WebUtility.HtmlDecode(obj.FirstOrDefault().languemat);
                        model.BesoinsSpecif = WebUtility.HtmlDecode(obj.FirstOrDefault().BesoinsSpecif);
                        model.AttentesGramm = obj.FirstOrDefault().AttentesGramm;
                        model.ConfrOral = obj.FirstOrDefault().ConfrOral;
                        model.nbabscprevue = obj.FirstOrDefault().nbabscprevue;
                        model.horaisouhait = WebUtility.HtmlDecode(obj.FirstOrDefault().horaisouhait);
                        model.sport = obj.FirstOrDefault().sport;
                        model.jardin = obj.FirstOrDefault().jardin;
                        model.musique = obj.FirstOrDefault().musique;
                        model.arts = obj.FirstOrDefault().arts;
                        model.litterature = obj.FirstOrDefault().litterature;
                        model.theatre = obj.FirstOrDefault().theatre;
                        model.cuisine = obj.FirstOrDefault().cuisine;
                        model.sciences = obj.FirstOrDefault().sciences;
                        model.LangGen = obj.FirstOrDefault().LangGen;
                        model.LangPro = obj.FirstOrDefault().LangPro;
                        model.litterature = obj.FirstOrDefault().litterature;
                        model.bricolage = obj.FirstOrDefault().bricolage;
                        model.ConfrLecture = obj.FirstOrDefault().ConfrLecture;
                        model.nbansbac = obj.FirstOrDefault().nbansbac;
                        model.formatAnt = WebUtility.HtmlDecode(obj.FirstOrDefault().formatAnt);
                        model.autres_interets = WebUtility.HtmlDecode(obj.FirstOrDefault().autres_interets);
                        model.Travelling_Lifestyle1 = obj.FirstOrDefault().Travelling_Lifestyle1;
                        model.Travelling_Lifestyle2 = obj.FirstOrDefault().Travelling_Lifestyle2;
                        model.Travelling_Lifestyle3 = obj.FirstOrDefault().Travelling_Lifestyle3;
                        model.Travelling_Lifestyle4 = obj.FirstOrDefault().Travelling_Lifestyle4;
                        model.Travelling_Lifestyle5 = obj.FirstOrDefault().Travelling_Lifestyle5;
                        model.Travelling_Lifestyle6 = obj.FirstOrDefault().Travelling_Lifestyle6;
                        model.Travelling_Lifestyle7 = obj.FirstOrDefault().Travelling_Lifestyle7;
                        model.Travelling_Lifestyle8 = obj.FirstOrDefault().Travelling_Lifestyle8;
                        model.Travelling_Lifestyle9 = obj.FirstOrDefault().Travelling_Lifestyle9;
                        model.Travelling_Lifestyle10 = obj.FirstOrDefault().Travelling_Lifestyle10;
                        model.Cinema_Arts1 = obj.FirstOrDefault().Cinema_Arts1;
                        model.Cinema_Arts2 = obj.FirstOrDefault().Cinema_Arts2;
                        model.Cinema_Arts3 = obj.FirstOrDefault().Cinema_Arts3;
                        model.Cinema_Arts4 = obj.FirstOrDefault().Cinema_Arts4;
                        model.Cinema_Arts5 = obj.FirstOrDefault().Cinema_Arts5;
                        model.Cinema_Arts6 = obj.FirstOrDefault().Cinema_Arts6;
                        model.Cinema_Arts7 = obj.FirstOrDefault().Cinema_Arts7;
                        model.Cinema_Arts8 = obj.FirstOrDefault().Cinema_Arts8;
                        model.Cinema_Arts9 = obj.FirstOrDefault().Cinema_Arts9;
                        model.Cinema_Arts10 = obj.FirstOrDefault().Cinema_Arts10;
                        model.Cinema_Arts11 = obj.FirstOrDefault().Cinema_Arts11;
                        model.Cinema_Arts12 = obj.FirstOrDefault().Cinema_Arts12;
                        model.Cinema_Arts13 = obj.FirstOrDefault().Cinema_Arts13;
                        model.Health_Fitness_Sport1 = obj.FirstOrDefault().Health_Fitness_Sport1;
                        model.Health_Fitness_Sport2 = obj.FirstOrDefault().Health_Fitness_Sport2;
                        model.Health_Fitness_Sport3 = obj.FirstOrDefault().Health_Fitness_Sport3;
                        model.Health_Fitness_Sport4 = obj.FirstOrDefault().Health_Fitness_Sport4;
                        model.Health_Fitness_Sport5 = obj.FirstOrDefault().Health_Fitness_Sport5;
                        model.Crafts_Hobbies1 = obj.FirstOrDefault().Crafts_Hobbies1;
                        model.Crafts_Hobbies2 = obj.FirstOrDefault().Crafts_Hobbies2;
                        model.Crafts_Hobbies3 = obj.FirstOrDefault().Crafts_Hobbies3;
                        model.Crafts_Hobbies4 = obj.FirstOrDefault().Crafts_Hobbies4;
                        model.Crafts_Hobbies5 = obj.FirstOrDefault().Crafts_Hobbies5;
                        model.Crafts_Hobbies6 = obj.FirstOrDefault().Crafts_Hobbies6;
                        model.Politics_Business1 = obj.FirstOrDefault().Politics_Business1;
                        model.Politics_Business2 = obj.FirstOrDefault().Politics_Business2;
                        model.Politics_Business3 = obj.FirstOrDefault().Politics_Business3;
                        model.Politics_Business4 = obj.FirstOrDefault().Politics_Business4;
                        model.Politics_Business5 = obj.FirstOrDefault().Politics_Business5;
                        model.Politics_Business6 = obj.FirstOrDefault().Politics_Business6;
                        model.Politics_Business7 = obj.FirstOrDefault().Politics_Business7;
                        model.Nature_Outdoors1 = obj.FirstOrDefault().Nature_Outdoors1;
                        model.Nature_Outdoors2 = obj.FirstOrDefault().Nature_Outdoors2;
                        model.Nature_Outdoors3 = obj.FirstOrDefault().Nature_Outdoors3;
                        model.Parenting_Family1 = obj.FirstOrDefault().Parenting_Family1;
                        model.Parenting_Family2 = obj.FirstOrDefault().Parenting_Family2;
                        model.Parenting_Family3 = obj.FirstOrDefault().Parenting_Family3;
                        model.Science_Technology1 = obj.FirstOrDefault().Science_Technology1;
                        model.Science_Technology2 = obj.FirstOrDefault().Science_Technology2;




                        return View(model);
                    }
                
                return View();
            }
            catch (Exception e)
            {

                return RedirectToAction("Notfound", "Home");
            }

        }

        [HttpGet]
        public ActionResult Monprofil(string q)
        {
            try
            {
                int id = TokenManager.ValidateToken(q);

                var Test = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, PROFIL.*, personal_interest.*, stagiaire_profil.numero FROM (stagiaire_profil LEFT JOIN personal_interest ON stagiaire_profil.numero = personal_interest.numero_profil) LEFT JOIN PROFIL ON stagiaire_profil.numero = PROFIL.numero WHERE (((stagiaire_profil.numero)=" + id + "));");
                if (Test.Count() == 0) { return RedirectToAction("Index", "Home"); }
                var compdate = "";
                if (Test != null)
                {
                    StagiaireModel models = new StagiaireModel();
                    models.nom = Test.FirstOrDefault().nom;
                    models.Numero = Test.FirstOrDefault().Numero;
                    compdate = models.Date_creation.Day.ToString();
                    compdate = models.nom;
                    compdate = compdate.Substring(0, 2); // Récupérer les deux premières lettres
                    compdate = compdate.ToUpper();
                }

               


                    // var obj = cnn.Query<StagiaireModel>(@"select S.numero,numindividu,civilite,fonction,telephone,nom,prenom,departement,portable,mail,societe,joursouhait,tempsprof,RueDuStage,VilleDuStage,CPDuStage,accesnetpro,horaisouhait,e_learning,accesnetperso,nbabscprevue,P.numero,niveausco,languemat,nbansbac,premlang,paysanglo,LangGen,LangPro,NationInterloc,FonctInterloc,BesoinsSpecif,AttentesSpec,formatAnt,AttentesGramm,AttentesCompreh,AttentesVocab,AttentesSpec,ConfrAccueilVisite,ConfrLecture,ConfrOral,ConfrPresent,ConfrNegoc,ObjStage,sport,jardin,musique,theatre,arts,sciences,litterature,bricolage,cuisine,autres_interets  from STAGIAIRE S right JOIN PROFIL P ON S.numero =P.numero where (S.numero=" + id + ");");
                    var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, PROFIL.*, personal_interest.*, stagiaire_profil.numero FROM (stagiaire_profil LEFT JOIN personal_interest ON stagiaire_profil.numero = personal_interest.numero_profil) LEFT JOIN PROFIL ON stagiaire_profil.numero = PROFIL.numero WHERE (((stagiaire_profil.numero)=" + id + "));");
                    if (obj.Count() == 0) { return RedirectToAction("Index", "Home"); }

                    if (obj != null)
                    {
                        StagiaireModel model = new StagiaireModel();

                        model.mail = obj.FirstOrDefault().mail;
                        model.fonction = WebUtility.HtmlDecode(obj.FirstOrDefault().fonction);
                        model.numindividu = obj.FirstOrDefault().numindividu;
                        model.Numero = obj.FirstOrDefault().Numero;
                        model.NationInterloc = obj.FirstOrDefault().NationInterloc;
                        model.nom = WebUtility.HtmlDecode(obj.FirstOrDefault().nom);
                        model.Portable = obj.FirstOrDefault().Portable;
                        model.premlang = WebUtility.HtmlDecode(obj.FirstOrDefault().premlang);
                        model.prenom = WebUtility.HtmlDecode(obj.FirstOrDefault().prenom);
                        model.Departement = WebUtility.HtmlDecode(obj.FirstOrDefault().Departement);
                        model.societe = WebUtility.HtmlDecode(obj.FirstOrDefault().societe);
                        model.Civilite = obj.FirstOrDefault().Civilite;
                        model.CPDuStage = WebUtility.HtmlDecode(obj.FirstOrDefault().CPDuStage);
                        model.tempsperso = obj.FirstOrDefault().tempsperso;
                        model.tempsprof = obj.FirstOrDefault().tempsprof;
                        model.RueDuStage = WebUtility.HtmlDecode(obj.FirstOrDefault().RueDuStage);
                        model.Telephone = obj.FirstOrDefault().Telephone;
                        model.joursouhait = WebUtility.HtmlDecode(obj.FirstOrDefault().joursouhait);
                        model.accesnetperso = obj.FirstOrDefault().accesnetperso;
                        model.accesnetpro = obj.FirstOrDefault().accesnetpro;
                        model.AttentesSpec = WebUtility.HtmlDecode(obj.FirstOrDefault().AttentesSpec);
                        model.E_learning = obj.FirstOrDefault().E_learning;
                        model.niveausco = obj.FirstOrDefault().niveausco;
                        model.VilleDuStage = WebUtility.HtmlDecode(obj.FirstOrDefault().VilleDuStage);
                        model.FonctInterloc = WebUtility.HtmlDecode(obj.FirstOrDefault().FonctInterloc);
                        model.paysanglo = WebUtility.HtmlDecode(obj.FirstOrDefault().paysanglo);
                        model.languemat = WebUtility.HtmlDecode(obj.FirstOrDefault().languemat);
                        model.BesoinsSpecif = WebUtility.HtmlDecode(obj.FirstOrDefault().BesoinsSpecif);
                        model.AttentesGramm = obj.FirstOrDefault().AttentesGramm;
                        model.ConfrOral = obj.FirstOrDefault().ConfrOral;
                        model.nbabscprevue = obj.FirstOrDefault().nbabscprevue;
                        model.horaisouhait = WebUtility.HtmlDecode(obj.FirstOrDefault().horaisouhait);
                        model.sport = obj.FirstOrDefault().sport;
                        model.jardin = obj.FirstOrDefault().jardin;
                        model.musique = obj.FirstOrDefault().musique;
                        model.arts = obj.FirstOrDefault().arts;
                        model.litterature = obj.FirstOrDefault().litterature;
                        model.theatre = obj.FirstOrDefault().theatre;
                        model.cuisine = obj.FirstOrDefault().cuisine;
                        model.sciences = obj.FirstOrDefault().sciences;
                        model.LangGen = obj.FirstOrDefault().LangGen;
                        model.LangPro = obj.FirstOrDefault().LangPro;
                        model.litterature = obj.FirstOrDefault().litterature;
                        model.bricolage = obj.FirstOrDefault().bricolage;
                        model.ConfrLecture = obj.FirstOrDefault().ConfrLecture;
                        model.nbansbac = obj.FirstOrDefault().nbansbac;
                        model.formatAnt = WebUtility.HtmlDecode(obj.FirstOrDefault().formatAnt);
                        model.autres_interets = WebUtility.HtmlDecode(obj.FirstOrDefault().autres_interets);
                        model.Travelling_Lifestyle1 = obj.FirstOrDefault().Travelling_Lifestyle1;
                        model.Travelling_Lifestyle2 = obj.FirstOrDefault().Travelling_Lifestyle2;
                        model.Travelling_Lifestyle3 = obj.FirstOrDefault().Travelling_Lifestyle3;
                        model.Travelling_Lifestyle4 = obj.FirstOrDefault().Travelling_Lifestyle4;
                        model.Travelling_Lifestyle5 = obj.FirstOrDefault().Travelling_Lifestyle5;
                        model.Travelling_Lifestyle6 = obj.FirstOrDefault().Travelling_Lifestyle6;
                        model.Travelling_Lifestyle7 = obj.FirstOrDefault().Travelling_Lifestyle7;
                        model.Travelling_Lifestyle8 = obj.FirstOrDefault().Travelling_Lifestyle8;
                        model.Travelling_Lifestyle9 = obj.FirstOrDefault().Travelling_Lifestyle9;
                        model.Travelling_Lifestyle10 = obj.FirstOrDefault().Travelling_Lifestyle10;
                        model.Cinema_Arts1 = obj.FirstOrDefault().Cinema_Arts1;
                        model.Cinema_Arts2 = obj.FirstOrDefault().Cinema_Arts2;
                        model.Cinema_Arts3 = obj.FirstOrDefault().Cinema_Arts3;
                        model.Cinema_Arts4 = obj.FirstOrDefault().Cinema_Arts4;
                        model.Cinema_Arts5 = obj.FirstOrDefault().Cinema_Arts5;
                        model.Cinema_Arts6 = obj.FirstOrDefault().Cinema_Arts6;
                        model.Cinema_Arts7 = obj.FirstOrDefault().Cinema_Arts7;
                        model.Cinema_Arts8 = obj.FirstOrDefault().Cinema_Arts8;
                        model.Cinema_Arts9 = obj.FirstOrDefault().Cinema_Arts9;
                        model.Cinema_Arts10 = obj.FirstOrDefault().Cinema_Arts10;
                        model.Cinema_Arts11 = obj.FirstOrDefault().Cinema_Arts11;
                        model.Cinema_Arts12 = obj.FirstOrDefault().Cinema_Arts12;
                        model.Cinema_Arts13 = obj.FirstOrDefault().Cinema_Arts13;
                        model.Health_Fitness_Sport1 = obj.FirstOrDefault().Health_Fitness_Sport1;
                        model.Health_Fitness_Sport2 = obj.FirstOrDefault().Health_Fitness_Sport2;
                        model.Health_Fitness_Sport3 = obj.FirstOrDefault().Health_Fitness_Sport3;
                        model.Health_Fitness_Sport4 = obj.FirstOrDefault().Health_Fitness_Sport4;
                        model.Health_Fitness_Sport5 = obj.FirstOrDefault().Health_Fitness_Sport5;
                        model.Crafts_Hobbies1 = obj.FirstOrDefault().Crafts_Hobbies1;
                        model.Crafts_Hobbies2 = obj.FirstOrDefault().Crafts_Hobbies2;
                        model.Crafts_Hobbies3 = obj.FirstOrDefault().Crafts_Hobbies3;
                        model.Crafts_Hobbies4 = obj.FirstOrDefault().Crafts_Hobbies4;
                        model.Crafts_Hobbies5 = obj.FirstOrDefault().Crafts_Hobbies5;
                        model.Crafts_Hobbies6 = obj.FirstOrDefault().Crafts_Hobbies6;
                        model.Politics_Business1 = obj.FirstOrDefault().Politics_Business1;
                        model.Politics_Business2 = obj.FirstOrDefault().Politics_Business2;
                        model.Politics_Business3 = obj.FirstOrDefault().Politics_Business3;
                        model.Politics_Business4 = obj.FirstOrDefault().Politics_Business4;
                        model.Politics_Business5 = obj.FirstOrDefault().Politics_Business5;
                        model.Politics_Business6 = obj.FirstOrDefault().Politics_Business6;
                        model.Politics_Business7 = obj.FirstOrDefault().Politics_Business7;
                        model.Nature_Outdoors1 = obj.FirstOrDefault().Nature_Outdoors1;
                        model.Nature_Outdoors2 = obj.FirstOrDefault().Nature_Outdoors2;
                        model.Nature_Outdoors3 = obj.FirstOrDefault().Nature_Outdoors3;
                        model.Parenting_Family1 = obj.FirstOrDefault().Parenting_Family1;
                        model.Parenting_Family2 = obj.FirstOrDefault().Parenting_Family2;
                        model.Parenting_Family3 = obj.FirstOrDefault().Parenting_Family3;
                        model.Science_Technology1 = obj.FirstOrDefault().Science_Technology1;
                        model.Science_Technology2 = obj.FirstOrDefault().Science_Technology2;




                        return View(model);
                    }
                
                else
                {
                    return RedirectToAction("Index", "Home");
                }


            }

            catch (Exception e)
            {

                return RedirectToAction("Index", "Home");
            }

        }
        [HttpGet]
        public ActionResult Monprof(int id)
        {
            try
            {



                // var obj = cnn.Query<StagiaireModel>(@"select S.numero,numindividu,civilite,fonction,telephone,nom,prenom,departement,portable,mail,societe,joursouhait,tempsprof,RueDuStage,VilleDuStage,CPDuStage,accesnetpro,horaisouhait,e_learning,accesnetperso,nbabscprevue,P.numero,niveausco,languemat,nbansbac,premlang,paysanglo,LangGen,LangPro,NationInterloc,FonctInterloc,BesoinsSpecif,AttentesSpec,formatAnt,AttentesGramm,AttentesCompreh,AttentesVocab,AttentesSpec,ConfrAccueilVisite,ConfrLecture,ConfrOral,ConfrPresent,ConfrNegoc,ObjStage,sport,jardin,musique,theatre,arts,sciences,litterature,bricolage,cuisine,autres_interets  from STAGIAIRE S right JOIN PROFIL P ON S.numero =P.numero where (S.numero=" + id + ");");
                var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, PROFIL.*, personal_interest.*, stagiaire_profil.numero FROM (stagiaire_profil LEFT JOIN personal_interest ON stagiaire_profil.numero = personal_interest.numero_profil) LEFT JOIN PROFIL ON stagiaire_profil.numero = PROFIL.numero WHERE (((stagiaire_profil.numero)=" + id + "));");
                if (obj.Count() == 0) { return RedirectToAction("Index", "Home"); }

                if (obj != null)
                {
                    StagiaireModel model = new StagiaireModel();

                    model.mail = obj.FirstOrDefault().mail;
                    model.fonction = WebUtility.HtmlDecode(obj.FirstOrDefault().fonction);
                    model.numindividu = obj.FirstOrDefault().numindividu;
                    model.Numero = obj.FirstOrDefault().Numero;
                    model.NationInterloc = obj.FirstOrDefault().NationInterloc;
                    model.nom = WebUtility.HtmlDecode(obj.FirstOrDefault().nom);
                    model.Portable = obj.FirstOrDefault().Portable;
                    model.premlang = WebUtility.HtmlDecode(obj.FirstOrDefault().premlang);
                    model.prenom = WebUtility.HtmlDecode(obj.FirstOrDefault().prenom);
                    model.Departement = WebUtility.HtmlDecode(obj.FirstOrDefault().Departement);
                    model.societe = WebUtility.HtmlDecode(obj.FirstOrDefault().societe);
                    model.Civilite = obj.FirstOrDefault().Civilite;
                    model.CPDuStage = WebUtility.HtmlDecode(obj.FirstOrDefault().CPDuStage);
                    model.tempsperso = obj.FirstOrDefault().tempsperso;
                    model.tempsprof = obj.FirstOrDefault().tempsprof;
                    model.RueDuStage = WebUtility.HtmlDecode(obj.FirstOrDefault().RueDuStage);
                    model.Telephone = obj.FirstOrDefault().Telephone;
                    model.joursouhait = WebUtility.HtmlDecode(obj.FirstOrDefault().joursouhait);
                    model.accesnetperso = obj.FirstOrDefault().accesnetperso;
                    model.accesnetpro = obj.FirstOrDefault().accesnetpro;
                    model.AttentesSpec = WebUtility.HtmlDecode(obj.FirstOrDefault().AttentesSpec);
                    model.E_learning = obj.FirstOrDefault().E_learning;
                    model.niveausco = obj.FirstOrDefault().niveausco;
                    model.VilleDuStage = WebUtility.HtmlDecode(obj.FirstOrDefault().VilleDuStage);
                    model.FonctInterloc = WebUtility.HtmlDecode(obj.FirstOrDefault().FonctInterloc);
                    model.paysanglo = WebUtility.HtmlDecode(obj.FirstOrDefault().paysanglo);
                    model.languemat = WebUtility.HtmlDecode(obj.FirstOrDefault().languemat);
                    model.BesoinsSpecif = WebUtility.HtmlDecode(obj.FirstOrDefault().BesoinsSpecif);
                    model.AttentesGramm = obj.FirstOrDefault().AttentesGramm;
                    model.ConfrOral = obj.FirstOrDefault().ConfrOral;
                    model.nbabscprevue = obj.FirstOrDefault().nbabscprevue;
                    model.horaisouhait = WebUtility.HtmlDecode(obj.FirstOrDefault().horaisouhait);
                    model.sport = obj.FirstOrDefault().sport;
                    model.jardin = obj.FirstOrDefault().jardin;
                    model.musique = obj.FirstOrDefault().musique;
                    model.arts = obj.FirstOrDefault().arts;
                    model.litterature = obj.FirstOrDefault().litterature;
                    model.theatre = obj.FirstOrDefault().theatre;
                    model.cuisine = obj.FirstOrDefault().cuisine;
                    model.sciences = obj.FirstOrDefault().sciences;
                    model.LangGen = obj.FirstOrDefault().LangGen;
                    model.LangPro = obj.FirstOrDefault().LangPro;
                    model.litterature = obj.FirstOrDefault().litterature;
                    model.bricolage = obj.FirstOrDefault().bricolage;
                    model.ConfrLecture = obj.FirstOrDefault().ConfrLecture;
                    model.nbansbac = obj.FirstOrDefault().nbansbac;
                    model.formatAnt = WebUtility.HtmlDecode(obj.FirstOrDefault().formatAnt);
                    model.autres_interets = WebUtility.HtmlDecode(obj.FirstOrDefault().autres_interets);
                    model.Travelling_Lifestyle1 = obj.FirstOrDefault().Travelling_Lifestyle1;
                    model.Travelling_Lifestyle2 = obj.FirstOrDefault().Travelling_Lifestyle2;
                    model.Travelling_Lifestyle3 = obj.FirstOrDefault().Travelling_Lifestyle3;
                    model.Travelling_Lifestyle4 = obj.FirstOrDefault().Travelling_Lifestyle4;
                    model.Travelling_Lifestyle5 = obj.FirstOrDefault().Travelling_Lifestyle5;
                    model.Travelling_Lifestyle6 = obj.FirstOrDefault().Travelling_Lifestyle6;
                    model.Travelling_Lifestyle7 = obj.FirstOrDefault().Travelling_Lifestyle7;
                    model.Travelling_Lifestyle8 = obj.FirstOrDefault().Travelling_Lifestyle8;
                    model.Travelling_Lifestyle9 = obj.FirstOrDefault().Travelling_Lifestyle9;
                    model.Travelling_Lifestyle10 = obj.FirstOrDefault().Travelling_Lifestyle10;
                    model.Cinema_Arts1 = obj.FirstOrDefault().Cinema_Arts1;
                    model.Cinema_Arts2 = obj.FirstOrDefault().Cinema_Arts2;
                    model.Cinema_Arts3 = obj.FirstOrDefault().Cinema_Arts3;
                    model.Cinema_Arts4 = obj.FirstOrDefault().Cinema_Arts4;
                    model.Cinema_Arts5 = obj.FirstOrDefault().Cinema_Arts5;
                    model.Cinema_Arts6 = obj.FirstOrDefault().Cinema_Arts6;
                    model.Cinema_Arts7 = obj.FirstOrDefault().Cinema_Arts7;
                    model.Cinema_Arts8 = obj.FirstOrDefault().Cinema_Arts8;
                    model.Cinema_Arts9 = obj.FirstOrDefault().Cinema_Arts9;
                    model.Cinema_Arts10 = obj.FirstOrDefault().Cinema_Arts10;
                    model.Cinema_Arts11 = obj.FirstOrDefault().Cinema_Arts11;
                    model.Cinema_Arts12 = obj.FirstOrDefault().Cinema_Arts12;
                    model.Cinema_Arts13 = obj.FirstOrDefault().Cinema_Arts13;
                    model.Health_Fitness_Sport1 = obj.FirstOrDefault().Health_Fitness_Sport1;
                    model.Health_Fitness_Sport2 = obj.FirstOrDefault().Health_Fitness_Sport2;
                    model.Health_Fitness_Sport3 = obj.FirstOrDefault().Health_Fitness_Sport3;
                    model.Health_Fitness_Sport4 = obj.FirstOrDefault().Health_Fitness_Sport4;
                    model.Health_Fitness_Sport5 = obj.FirstOrDefault().Health_Fitness_Sport5;
                    model.Crafts_Hobbies1 = obj.FirstOrDefault().Crafts_Hobbies1;
                    model.Crafts_Hobbies2 = obj.FirstOrDefault().Crafts_Hobbies2;
                    model.Crafts_Hobbies3 = obj.FirstOrDefault().Crafts_Hobbies3;
                    model.Crafts_Hobbies4 = obj.FirstOrDefault().Crafts_Hobbies4;
                    model.Crafts_Hobbies5 = obj.FirstOrDefault().Crafts_Hobbies5;
                    model.Crafts_Hobbies6 = obj.FirstOrDefault().Crafts_Hobbies6;
                    model.Politics_Business1 = obj.FirstOrDefault().Politics_Business1;
                    model.Politics_Business2 = obj.FirstOrDefault().Politics_Business2;
                    model.Politics_Business3 = obj.FirstOrDefault().Politics_Business3;
                    model.Politics_Business4 = obj.FirstOrDefault().Politics_Business4;
                    model.Politics_Business5 = obj.FirstOrDefault().Politics_Business5;
                    model.Politics_Business6 = obj.FirstOrDefault().Politics_Business6;
                    model.Politics_Business7 = obj.FirstOrDefault().Politics_Business7;
                    model.Nature_Outdoors1 = obj.FirstOrDefault().Nature_Outdoors1;
                    model.Nature_Outdoors2 = obj.FirstOrDefault().Nature_Outdoors2;
                    model.Nature_Outdoors3 = obj.FirstOrDefault().Nature_Outdoors3;
                    model.Parenting_Family1 = obj.FirstOrDefault().Parenting_Family1;
                    model.Parenting_Family2 = obj.FirstOrDefault().Parenting_Family2;
                    model.Parenting_Family3 = obj.FirstOrDefault().Parenting_Family3;
                    model.Science_Technology1 = obj.FirstOrDefault().Science_Technology1;
                    model.Science_Technology2 = obj.FirstOrDefault().Science_Technology2;




                    return View(model);
                }

                else
                {
                    return RedirectToAction("Index", "Home");
                }


            }

            catch (Exception e)
            {

                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost]
        public ActionResult EnregistrerEl()

        {
            var model = new StagiaireModel();

            int numero;

            // Tenter de convertir la valeur en int
            if (int.TryParse(Request.Form["numeroprofil"], out numero))
            {
                // Conversion réussie, assignez la valeur à la propriété de votre modèle
                model.Numero = numero;
            }
            else
            {
                // Conversion échouée, attribuez une valeur par défaut ou traitez l'erreur
                // Par exemple, attribuer une valeur par défaut à model.Numero
                model.Numero = 0; // Ou une autre valeur par défaut appropriée pour votre application
            }
            int numindiv;

            // Tenter de convertir la valeur en int
            if (int.TryParse(Request.Form["numindiv"], out numindiv))
            {
                // Conversion réussie, assignez la valeur à la propriété de votre modèle
                model.numindividu = numindiv;
            }
            else
            {
                // Conversion échouée, attribuez une valeur par défaut ou traitez l'erreur
                // Par exemple, attribuer une valeur par défaut à model.Numero
                model.numindividu = 0; // Ou une autre valeur par défaut appropriée pour votre application
            }
            bool accesNetPerso;

            // Tenter de convertir la valeur en booléen
            if (bool.TryParse(Request.Form["accesnetperso"], out accesNetPerso))
            {
                // Conversion réussie, assignez la valeur à la propriété de votre modèle
                model.accesnetperso = accesNetPerso;
            }
            else
            {
                // Conversion échouée, attribuez une valeur par défaut ou traitez l'erreur
                // Par exemple, attribuer une valeur par défaut à model.AccesNetPerso
                model.accesnetperso = false; // Ou une autre valeur par défaut appropriée pour votre application
            }

            bool accesnetPro;

            // Tenter de convertir la valeur en booléen
            if (bool.TryParse(Request.Form["accesnetpro"], out accesnetPro))
            {
                // Conversion réussie, assignez la valeur à la propriété de votre modèle
                model.accesnetpro = accesnetPro;
            }
            else
            {
                // Conversion échouée, attribuez une valeur par défaut ou traitez l'erreur
                // Par exemple, attribuer une valeur par défaut à model.AccesNetPerso
                model.accesnetpro = false; // Ou une autre valeur par défaut appropriée pour votre application
            }
            bool sport;

            // Tenter de convertir la valeur en booléen
            if (bool.TryParse(Request.Form["sport"], out sport))
            {
                // Conversion réussie, assignez la valeur à la propriété de votre modèle
                model.sport = sport;
            }
            else
            {
                // Conversion échouée, attribuez une valeur par défaut ou traitez l'erreur
                // Par exemple, attribuer une valeur par défaut à model.AccesNetPerso
                model.sport = false; // Ou une autre valeur par défaut appropriée pour votre application
            }
            bool jardin;

            // Tenter de convertir la valeur en booléen
            if (bool.TryParse(Request.Form["jardin"], out jardin))
            {
                // Conversion réussie, assignez la valeur à la propriété de votre modèle
                model.jardin = jardin;
            }
            else
            {
                // Conversion échouée, attribuez une valeur par défaut ou traitez l'erreur
                // Par exemple, attribuer une valeur par défaut à model.AccesNetPerso
                model.jardin = false; // Ou une autre valeur par défaut appropriée pour votre application
            }
            bool musique;

            // Tenter de convertir la valeur en booléen
            if (bool.TryParse(Request.Form["musique"], out musique))
            {
                // Conversion réussie, assignez la valeur à la propriété de votre modèle
                model.musique = musique;
            }
            else
            {
                // Conversion échouée, attribuez une valeur par défaut ou traitez l'erreur
                // Par exemple, attribuer une valeur par défaut à model.AccesNetPerso
                model.musique = false; // Ou une autre valeur par défaut appropriée pour votre application
            }


            model.mail = Request.Form["mail"];
            model.fonction = WebUtility.HtmlDecode(Request.Form["fonction"]);
            model.numindividu = numindiv;
            model.Numero = numero;
            model.NationInterloc = Request.Form["NationInterloc"];
            model.nom = Request.Form["nom"];
            model.Portable = Request.Form["Portable"];
            model.premlang = WebUtility.HtmlDecode(Request.Form["premlang"]);
            model.prenom = Request.Form["prenom"];
            model.Departement = WebUtility.HtmlDecode(Request.Form["Departement"]);
            model.societe = WebUtility.HtmlDecode(Request.Form["societe"]);
            model.Civilite = Request.Form["Civilite"];
            model.CPDuStage = WebUtility.HtmlDecode(Request.Form["CPDuStage"]);
            model.tempsperso = Request.Form["tempsperso"];
            model.tempsprof = Request.Form["tempsprof"];
            model.RueDuStage = WebUtility.HtmlDecode(Request.Form["RueDuStage"]);
            model.Telephone = Request.Form["Telephone"];
            model.joursouhait = WebUtility.HtmlDecode(Request.Form["joursouhait"]);
            model.accesnetperso = accesNetPerso;
            model.accesnetpro = accesnetPro;
            model.AttentesSpec = WebUtility.HtmlDecode(Request.Form["AttentesSpec"]);
            model.VilleDuStage = WebUtility.HtmlDecode(Request.Form["VilleDuStage"]);
            model.FonctInterloc = WebUtility.HtmlDecode(Request.Form["FonctInterloc"]);
            model.paysanglo = WebUtility.HtmlDecode(Request.Form["paysanglo"]);
            model.languemat = WebUtility.HtmlDecode(Request.Form["languemat"]);
            model.BesoinsSpecif = WebUtility.HtmlDecode(Request.Form["BesoinsSpecif"]);
            model.nbabscprevue = Request.Form["nbabscprevue"];
            model.horaisouhait = WebUtility.HtmlDecode(Request.Form["horaisouhait"]);
            model.sport = sport;
            model.jardin = jardin;
            model.musique = musique;
            bool arts;

            // Tenter de convertir la valeur en booléen
            if (bool.TryParse(Request.Form["arts"], out arts))
            {
                // Conversion réussie, assignez la valeur à la propriété de votre modèle
                model.arts = arts;
            }
            else
            {
                // Conversion échouée, attribuez une valeur par défaut ou traitez l'erreur
                // Par exemple, attribuer une valeur par défaut à model.AccesNetPerso
                model.arts = false; // Ou une autre valeur par défaut appropriée pour votre application
            }

            model.arts = arts;
            bool litterature;

            // Tenter de convertir la valeur en booléen
            if (bool.TryParse(Request.Form["litterature"], out litterature))
            {
                // Conversion réussie, assignez la valeur à la propriété de votre modèle
                model.litterature = litterature;
            }
            else
            {
                // Conversion échouée, attribuez une valeur par défaut ou traitez l'erreur
                // Par exemple, attribuer une valeur par défaut à model.AccesNetPerso
                model.litterature = false; // Ou une autre valeur par défaut appropriée pour votre application
            }

            model.litterature = litterature;
            bool theatre;

            // Tenter de convertir la valeur en booléen
            if (bool.TryParse(Request.Form["theatre"], out theatre))
            {
                // Conversion réussie, assignez la valeur à la propriété de votre modèle
                model.theatre = theatre;
            }
            else
            {
                // Conversion échouée, attribuez une valeur par défaut ou traitez l'erreur
                // Par exemple, attribuer une valeur par défaut à model.AccesNetPerso
                model.theatre = false; // Ou une autre valeur par défaut appropriée pour votre application
            }

            model.theatre = theatre;

            bool cuisine;

            // Tenter de convertir la valeur en booléen
            if (bool.TryParse(Request.Form["cuisine"], out cuisine))
            {
                // Conversion réussie, assignez la valeur à la propriété de votre modèle
                model.cuisine = cuisine;
            }
            else
            {
                // Conversion échouée, attribuez une valeur par défaut ou traitez l'erreur
                // Par exemple, attribuer une valeur par défaut à model.AccesNetPerso
                model.cuisine = false; // Ou une autre valeur par défaut appropriée pour votre application
            }


            model.cuisine = cuisine;

            bool sciences;

            // Tenter de convertir la valeur en booléen
            if (bool.TryParse(Request.Form["sciences"], out sciences))
            {
                // Conversion réussie, assignez la valeur à la propriété de votre modèle
                model.sciences = sciences;
            }
            else
            {
                // Conversion échouée, attribuez une valeur par défaut ou traitez l'erreur
                // Par exemple, attribuer une valeur par défaut à model.AccesNetPerso
                model.sciences = false; // Ou une autre valeur par défaut appropriée pour votre application
            }

            model.sciences = sciences;
            bool LangGen;

            // Tenter de convertir la valeur en booléen
            if (bool.TryParse(Request.Form["LangGen"], out LangGen))
            {
                // Conversion réussie, assignez la valeur à la propriété de votre modèle
                model.LangGen = LangGen;
            }
            else
            {
                // Conversion échouée, attribuez une valeur par défaut ou traitez l'erreur
                // Par exemple, attribuer une valeur par défaut à model.AccesNetPerso
                model.LangGen = false; // Ou une autre valeur par défaut appropriée pour votre application
            }


            model.LangGen = LangGen;

            bool LangPro;

            // Tenter de convertir la valeur en booléen
            if (bool.TryParse(Request.Form["LangPro"], out LangPro))
            {
                // Conversion réussie, assignez la valeur à la propriété de votre modèle
                model.LangPro = LangPro;
            }
            else
            {
                // Conversion échouée, attribuez une valeur par défaut ou traitez l'erreur
                // Par exemple, attribuer une valeur par défaut à model.AccesNetPerso
                model.LangPro = false; // Ou une autre valeur par défaut appropriée pour votre application
            }
            bool bricolage;

            // Tenter de convertir la valeur en booléen
            if (bool.TryParse(Request.Form["bricolage"], out bricolage))
            {
                // Conversion réussie, assignez la valeur à la propriété de votre modèle
                model.bricolage = bricolage;
            }
            else
            {
                // Conversion échouée, attribuez une valeur par défaut ou traitez l'erreur
                // Par exemple, attribuer une valeur par défaut à model.AccesNetPerso
                model.bricolage = false; // Ou une autre valeur par défaut appropriée pour votre application
            }
            bool ConfrLecture;

            // Tenter de convertir la valeur en booléen
            if (bool.TryParse(Request.Form["ConfrLecture"], out ConfrLecture))
            {
                // Conversion réussie, assignez la valeur à la propriété de votre modèle
                model.ConfrLecture = ConfrLecture;
            }
            else
            {
                // Conversion échouée, attribuez une valeur par défaut ou traitez l'erreur
                // Par exemple, attribuer une valeur par défaut à model.AccesNetPerso
                model.ConfrLecture = false; // Ou une autre valeur par défaut appropriée pour votre application
            }
            model.LangPro = LangPro;
            model.bricolage = bricolage;
            model.ConfrLecture = ConfrLecture;
            model.nbansbac = Request.Form["nbansbac"];
            model.formatAnt = WebUtility.HtmlDecode(Request.Form["formatAnt"]);
            model.autres_interets = WebUtility.HtmlDecode(Request.Form["autres_interets"]);

            // Faire ce que vous voulez avec le modèle ici

            return Monprofil(model); // Retourner une vue avec le modèle
        }
        [HttpGet]
        public ActionResult Editprofil(int id, string datecrea)
        {
            try
            {
                var Test = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, PROFIL.*, personal_interest.*, stagiaire_profil.numero FROM (stagiaire_profil LEFT JOIN personal_interest ON stagiaire_profil.numero = personal_interest.numero_profil) LEFT JOIN PROFIL ON stagiaire_profil.numero = PROFIL.numero WHERE (((stagiaire_profil.numero)="+ id +" )) ;");
                if (Test.Count() == 0) { return RedirectToAction("Index", "Home"); }
                var compdate="";
                if (Test != null)
                {
                    StagiaireModel models = new StagiaireModel();
                    models.nom = Test.FirstOrDefault().nom;
                    models.Numero = Test.FirstOrDefault().Numero;
                    compdate = models.Date_creation.Day.ToString();
                    compdate = models.nom ;
                     compdate = compdate.Substring(0, 2); // Récupérer les deux premières lettres
                    compdate = compdate.ToUpper();
                }

                 datecrea = datecrea.Substring(0, 2); // Récupérer les deux premières lettres
                datecrea = datecrea.ToUpper();
                if (compdate == datecrea)
                {

               
                    // var obj = cnn.Query<StagiaireModel>(@"select S.numero,numindividu,civilite,fonction,telephone,nom,prenom,departement,portable,mail,societe,joursouhait,tempsprof,RueDuStage,VilleDuStage,CPDuStage,accesnetpro,horaisouhait,e_learning,accesnetperso,nbabscprevue,P.numero,niveausco,languemat,nbansbac,premlang,paysanglo,LangGen,LangPro,NationInterloc,FonctInterloc,BesoinsSpecif,AttentesSpec,formatAnt,AttentesGramm,AttentesCompreh,AttentesVocab,AttentesSpec,ConfrAccueilVisite,ConfrLecture,ConfrOral,ConfrPresent,ConfrNegoc,ObjStage,sport,jardin,musique,theatre,arts,sciences,litterature,bricolage,cuisine,autres_interets  from STAGIAIRE S right JOIN PROFIL P ON S.numero =P.numero where (S.numero=" + id + ");");
                    var obj = cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, PROFIL.*, personal_interest.*, stagiaire_profil.numero FROM (stagiaire_profil LEFT JOIN personal_interest ON stagiaire_profil.numero = personal_interest.numero_profil) LEFT JOIN PROFIL ON stagiaire_profil.numero = PROFIL.numero WHERE (((stagiaire_profil.numero)="+ id +")) ;");
                    if (obj.Count() == 0) { return RedirectToAction("Index", "Home"); }

                    if (obj != null)
                    {
                        StagiaireModel model = new StagiaireModel();

                        model.mail = obj.FirstOrDefault().mail;
                        model.fonction = WebUtility.HtmlDecode(obj.FirstOrDefault().fonction);
                        model.numindividu = obj.FirstOrDefault().numindividu;
                        model.Numero = obj.FirstOrDefault().Numero;
                        model.NationInterloc = obj.FirstOrDefault().NationInterloc;
                        model.nom = WebUtility.HtmlDecode(obj.FirstOrDefault().nom);
                        model.Portable = obj.FirstOrDefault().Portable;
                        model.premlang = WebUtility.HtmlDecode(obj.FirstOrDefault().premlang);
                        model.prenom = WebUtility.HtmlDecode(obj.FirstOrDefault().prenom);
                        model.Departement = WebUtility.HtmlDecode(obj.FirstOrDefault().Departement);
                        model.societe = WebUtility.HtmlDecode(obj.FirstOrDefault().societe);
                        model.Civilite = obj.FirstOrDefault().Civilite;
                        model.CPDuStage = WebUtility.HtmlDecode(obj.FirstOrDefault().CPDuStage);
                        model.tempsperso = obj.FirstOrDefault().tempsperso;
                        model.tempsprof = obj.FirstOrDefault().tempsprof;
                        model.RueDuStage = WebUtility.HtmlDecode(obj.FirstOrDefault().RueDuStage);
                        model.Telephone = obj.FirstOrDefault().Telephone;
                        model.joursouhait = WebUtility.HtmlDecode(obj.FirstOrDefault().joursouhait);
                        model.accesnetperso = obj.FirstOrDefault().accesnetperso;
                        model.accesnetpro = obj.FirstOrDefault().accesnetpro;
                        model.AttentesSpec = WebUtility.HtmlDecode(obj.FirstOrDefault().AttentesSpec);
                        model.E_learning = obj.FirstOrDefault().E_learning;
                        model.niveausco = obj.FirstOrDefault().niveausco;
                        model.VilleDuStage = WebUtility.HtmlDecode(obj.FirstOrDefault().VilleDuStage);
                        model.FonctInterloc = WebUtility.HtmlDecode(obj.FirstOrDefault().FonctInterloc);
                        model.paysanglo = WebUtility.HtmlDecode(obj.FirstOrDefault().paysanglo);
                        model.languemat = WebUtility.HtmlDecode(obj.FirstOrDefault().languemat);
                        model.BesoinsSpecif = WebUtility.HtmlDecode(obj.FirstOrDefault().BesoinsSpecif);
                        model.AttentesGramm = obj.FirstOrDefault().AttentesGramm;
                        model.ConfrOral = obj.FirstOrDefault().ConfrOral;
                        model.nbabscprevue = obj.FirstOrDefault().nbabscprevue;
                        model.horaisouhait = WebUtility.HtmlDecode(obj.FirstOrDefault().horaisouhait);
                        model.sport = obj.FirstOrDefault().sport;
                        model.jardin = obj.FirstOrDefault().jardin;
                        model.musique = obj.FirstOrDefault().musique;
                        model.arts = obj.FirstOrDefault().arts;
                        model.litterature = obj.FirstOrDefault().litterature;
                        model.theatre = obj.FirstOrDefault().theatre;
                        model.cuisine = obj.FirstOrDefault().cuisine;
                        model.sciences = obj.FirstOrDefault().sciences;
                        model.LangGen = obj.FirstOrDefault().LangGen;
                        model.LangPro = obj.FirstOrDefault().LangPro;
                        model.litterature = obj.FirstOrDefault().litterature;
                        model.bricolage = obj.FirstOrDefault().bricolage;
                        model.ConfrLecture = obj.FirstOrDefault().ConfrLecture;
                        model.nbansbac = obj.FirstOrDefault().nbansbac;
                        model.formatAnt = WebUtility.HtmlDecode(obj.FirstOrDefault().formatAnt);
                        model.autres_interets = WebUtility.HtmlDecode(obj.FirstOrDefault().autres_interets);
                        model.Travelling_Lifestyle1 = obj.FirstOrDefault().Travelling_Lifestyle1;
                        model.Travelling_Lifestyle2 = obj.FirstOrDefault().Travelling_Lifestyle2;
                        model.Travelling_Lifestyle3 = obj.FirstOrDefault().Travelling_Lifestyle3;
                        model.Travelling_Lifestyle4 = obj.FirstOrDefault().Travelling_Lifestyle4;
                        model.Travelling_Lifestyle5 = obj.FirstOrDefault().Travelling_Lifestyle5;
                        model.Travelling_Lifestyle6 = obj.FirstOrDefault().Travelling_Lifestyle6;
                        model.Travelling_Lifestyle7 = obj.FirstOrDefault().Travelling_Lifestyle7;
                        model.Travelling_Lifestyle8 = obj.FirstOrDefault().Travelling_Lifestyle8;
                        model.Travelling_Lifestyle9 = obj.FirstOrDefault().Travelling_Lifestyle9;
                        model.Travelling_Lifestyle10 = obj.FirstOrDefault().Travelling_Lifestyle10;
                        model.Cinema_Arts1 = obj.FirstOrDefault().Cinema_Arts1;
                        model.Cinema_Arts2 = obj.FirstOrDefault().Cinema_Arts2;
                        model.Cinema_Arts3 = obj.FirstOrDefault().Cinema_Arts3;
                        model.Cinema_Arts4 = obj.FirstOrDefault().Cinema_Arts4;
                        model.Cinema_Arts5 = obj.FirstOrDefault().Cinema_Arts5;
                        model.Cinema_Arts6 = obj.FirstOrDefault().Cinema_Arts6;
                        model.Cinema_Arts7 = obj.FirstOrDefault().Cinema_Arts7;
                        model.Cinema_Arts8 = obj.FirstOrDefault().Cinema_Arts8;
                        model.Cinema_Arts9 = obj.FirstOrDefault().Cinema_Arts9;
                        model.Cinema_Arts10 = obj.FirstOrDefault().Cinema_Arts10;
                        model.Cinema_Arts11 = obj.FirstOrDefault().Cinema_Arts11;
                        model.Cinema_Arts12 = obj.FirstOrDefault().Cinema_Arts12;
                        model.Cinema_Arts13 = obj.FirstOrDefault().Cinema_Arts13;
                        model.Health_Fitness_Sport1 = obj.FirstOrDefault().Health_Fitness_Sport1;
                        model.Health_Fitness_Sport2 = obj.FirstOrDefault().Health_Fitness_Sport2;
                        model.Health_Fitness_Sport3 = obj.FirstOrDefault().Health_Fitness_Sport3;
                        model.Health_Fitness_Sport4 = obj.FirstOrDefault().Health_Fitness_Sport4;
                        model.Health_Fitness_Sport5 = obj.FirstOrDefault().Health_Fitness_Sport5;
                        model.Crafts_Hobbies1 = obj.FirstOrDefault().Crafts_Hobbies1;
                        model.Crafts_Hobbies2 = obj.FirstOrDefault().Crafts_Hobbies2;
                        model.Crafts_Hobbies3 = obj.FirstOrDefault().Crafts_Hobbies3;
                        model.Crafts_Hobbies4 = obj.FirstOrDefault().Crafts_Hobbies4;
                        model.Crafts_Hobbies5 = obj.FirstOrDefault().Crafts_Hobbies5;
                        model.Crafts_Hobbies6 = obj.FirstOrDefault().Crafts_Hobbies6;
                        model.Politics_Business1 = obj.FirstOrDefault().Politics_Business1;
                        model.Politics_Business2 = obj.FirstOrDefault().Politics_Business2;
                        model.Politics_Business3 = obj.FirstOrDefault().Politics_Business3;
                        model.Politics_Business4 = obj.FirstOrDefault().Politics_Business4;
                        model.Politics_Business5 = obj.FirstOrDefault().Politics_Business5;
                        model.Politics_Business6 = obj.FirstOrDefault().Politics_Business6;
                        model.Politics_Business7 = obj.FirstOrDefault().Politics_Business7;
                        model.Nature_Outdoors1 = obj.FirstOrDefault().Nature_Outdoors1;
                        model.Nature_Outdoors2 = obj.FirstOrDefault().Nature_Outdoors2;
                        model.Nature_Outdoors3 = obj.FirstOrDefault().Nature_Outdoors3;
                        model.Parenting_Family1 = obj.FirstOrDefault().Parenting_Family1;
                        model.Parenting_Family2 = obj.FirstOrDefault().Parenting_Family2;
                        model.Parenting_Family3 = obj.FirstOrDefault().Parenting_Family3;
                        model.Science_Technology1 = obj.FirstOrDefault().Science_Technology1;
                        model.Science_Technology2 = obj.FirstOrDefault().Science_Technology2;
                        var listeDesSocietes = StagiaireModel.GetAllSocietes();

                        // Assigner la liste des sociétés au modèle
                        model.ListeDesSocietes = listeDesSocietes;

                      //  model.nom_change = obj.FirstOrDefault().nom;

                        return View(model);
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                
                
                return View();
            }

            catch (Exception e)
            {

                return RedirectToAction("Notfound", "Home");
            }

        }
        public async Task<ActionResult> YourAction(StagiaireModel model)
        {
            //nvert the model to XML
            string Getxml = GetXmlStringFromModel(model);
            string url = $"{GetAutoPlannificationLink()}/webcalendar/srcs/www/index.php?module=auto&action=import:index&PostedXMLStagiaire={Getxml}";


            // Send the XML data and receive a JSON response
        //    string jsonResponse = await SendXmlDataAndGetJsonResponse(apiUrl, strXMLStagiaire);
            string jsonResponse = await SendXmlDataAndGetJsonResponse(url, Getxml);

            // Your other logic here...
            var jsonObject = JsonConvert.DeserializeObject(jsonResponse);

            return Json(jsonObject);
        }
        public ActionResult GenerateTokenLogin(int id)
        {
            // Générer le token en utilisant uniquement l'ID
            string token = TokenManager.GenerateToken(id);

            // Créer un objet JSON avec le token
            var jsonResponse = new { T = token };

            // Retourner une réponse JSON
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Monprofil(StagiaireModel model)
        {
            //  var obj = cnn.Execute("update STAGIAIRE set [fonction]='" + model.Fonction + "', [mail]='" + model.Mail + "',[civilite]='" + model.Civilite + "',[telephone]='" + model.Telephone + "',[nom]='" + model.Nom + "',[prenom]='" + model.Prenom + "',[departement]='" + model.Departement + "',[portable]='" + model.Portable + "',[societe]='" + model.Societe + "',[joursouhait]='" + model.Joursouhait + "',[tempsprof]='" + model.Tempsprof + "',[RueDuStage]='" + model.RueDuStage + "',[VilleDuStage]='" + model.VilleDuStage + "',[CPDuStage]='" + model.CPDuStage + "',[accesnetpro]=" + model.Accesnetpro + ",[horaisouhait]='" + model.Horaisouhait + "',[e_learning]=" + model.E_learning + ",[accesnetperso]=" + model.Accesnetperso + ",[nbabscprevue]='" + model.Nbabscprevue + "'  where numero =" + model.Numero + " ");
            //  var obj1 = cnn.Execute("update PROFIL set [niveausco]=" + model.NiveauSco + " ,[languemat]='" + model.Languemat + "',[nbansbac]='" + model.Nbansbac + "',[premlang]='" + model.Premlang + "',[paysanglo]='" + model.paysanglo + "',[LangGen]=" + model.LangGen + ",[LangPro]=" + model.LangPro+ ",[NationInterloc]='" + model.NationInterloc + "',[FonctInterloc]='" + model.FonctInterloc + "',[BesoinsSpecif]='" + model.BesoinsSpecif + "',[AttentesSpec]='" + model.AttentesSpec + "',[formatAnt]='" + model.FormatAnt + "',[ConfrAccueilVisite]=" + model.ConfrAccueilVisite + ",[ConfrLecture]=" + model.ConfrLecture + ",[ConfrOral]=" + model.ConfrOral + ",[ConfrPresent]=" + model.ConfrPresent + ",[ConfrNegoc]=" + model.ConfrNegoc + ",[ObjStage]='" + model.ObjStage + "',[sport]=" + model.Sport + ",[jardin]=" + model.Jardin + ",[musique]=" + model.Musique + ",[theatre]=" + model.Theatre + ",[arts]=" + model.Arts + ",[sciences]=" + model.Sciences + ",[litterature]=" + model.Litterature + ",[bricolage]=" + model.Bricolage + ",[cuisine]=" + model.Cuisine + ",[autres_interets]='" + model.Autres_interets + "' where numero =" + model.Numero + " ");
            //  StagiaireModel model = new StagiaireModel();
            try
            {
                if (ModelState.IsValid)
                {

                    string attentes, besoins, villestage, ruestage, centres_interets, nom, prenom, societe, nbascence, fonction, departement, codepostal, langmat, premierlang, sejour, leurfonct, leurnational, domaine, autres;

                    fonction = conversion(model.fonction);
                    departement = conversion(model.Departement);
                    codepostal = conversion(model.CPDuStage);
                    langmat = conversion(model.languemat);
                    premierlang = conversion(model.premlang);
                    sejour = conversion(model.paysanglo);
                    leurfonct = conversion(model.FonctInterloc);
                    leurnational = conversion(model.NationInterloc);
                    domaine = conversion(model.AttentesSpec);
                    autres = conversion(model.autres_interets);
                    nom = WebUtility.HtmlDecode(model.nom);
                    if (!(string.IsNullOrEmpty(nom)))
                    {
                        nom = nom.Replace("'", "''");
                    }
                    if ((string.IsNullOrEmpty(fonction)))
                    {
                        fonction = ".";
                    }
                    nbascence = WebUtility.HtmlDecode(model.nbansbac);
                    if (!(string.IsNullOrEmpty(nbascence)))
                    {
                        nbascence = nbascence.Replace("'", "''");
                    }
                    societe = WebUtility.HtmlDecode(model.societe);
                    if (!(string.IsNullOrEmpty(societe)))
                    {
                        societe = societe.Replace("'", "''");
                    }
                    prenom = WebUtility.HtmlDecode(model.prenom);
                    if (!(string.IsNullOrEmpty(prenom)))
                    {
                        prenom = prenom.Replace("'", "''");
                    }
                    attentes = conversion((model.AttentesSpec));
                    if ((string.IsNullOrEmpty(attentes)))
                    {
                        attentes = ".";
                    }
                    besoins = conversion(model.BesoinsSpecif);
                    // besoins = WebUtility.HtmlDecode(model.AttentesSpec);
                    if ((string.IsNullOrEmpty(besoins)))
                    {
                        besoins = ".";
                    }
                    villestage = conversion(model.VilleDuStage);


                    ruestage = conversion(model.RueDuStage);
                    if (!(string.IsNullOrEmpty(ruestage)))
                    {
                        ruestage = ruestage.Replace("'", "''");
                        ruestage = ruestage.Replace("&", "");
                        ruestage = ruestage.Replace("#", "");
                        ruestage = ruestage.Replace(";", "");

                    }

                    centres_interets = conversion(model.autres_interets);
                    if (!(string.IsNullOrEmpty(centres_interets)))
                    {
                        centres_interets = centres_interets.Replace("'", "''");
                    }
                    else
                    {
                        centres_interets = ".";
                    }
                    var champvide = 0;
                    var telph1 = model.Portable;
                    if (string.IsNullOrEmpty(model.Portable))
                    {
                        telph1 = conversion(model.Portable);
                        champvide = 1;
                    }

                    var telph2 = model.Telephone;

                    if (string.IsNullOrEmpty(telph2) || telph2 == ".")
                    {
                        telph2 = telph1;
                    }

                    var temps_prof = conversion(model.tempsprof);

                    var temps_perso = conversion(model.tempsperso);

                    var jour_essa = conversion(model.joursouhait);


                    var nbs_absencpreve = conversion(model.nbabscprevue);

                    var nbs_absence = conversion(model.nbansbac);

                    var formant = conversion(model.formatAnt);
                    var horairesouhait = conversion(model.horaisouhait);
                    var numind = model.numindividu;


                    // Check if model.numindividu is nequal to 0 and not null

                    /*    string obtest = "SELECT stagiaire_profil.numero, stagiaire_profil.nom, stagiaire_profil.numindividu FROM stagiaire_profil WHERE(((stagiaire_profil.numero) = "+model.Numero+"))";
                        var objs = cnn.Query<StagiaireModel>(obtest);
                        if (objs != null)
                        {
                            StagiaireModel nombre = new StagiaireModel();

                            nombre.numindividu = objs.FirstOrDefault().numindividu;

                        }*/
                    if (codepostal == ".")
                    {
                        codepostal = "0";
                    }
                    var obj = cnn.Execute("update stagiaire_profil set [DateDerModif]=now(),[fonction]='" + fonction + "', [mail]='" + model.mail + "',[civilite]='" + model.Civilite + "',[telephone]='" + telph2 + "',[nom]='" + nom + "',[prenom]='" + prenom + "',[departement]='" + departement + "',[portable]='" + telph1 + "',[joursouhait]='" + jour_essa + "',[tempsprof]='" + temps_prof + "',[tempsperso]='" + temps_perso + "',[RueDuStage]='" + ruestage + "',[VilleDuStage]='" + villestage + "',[CPDuStage]='" + codepostal + "',[accesnetpro]=" + model.accesnetpro + ",[horaisouhait]='" + horairesouhait + "',[e_learning]=" + model.E_learning + ",[accesnetperso]=" + model.accesnetperso + ",[nbabscprevue]='" + nbs_absencpreve + "', [societe]='" + model.societe + "'  where numero =" + model.Numero + " ");
                    //  string sql2 = "update Individus set[Civilité] = '" + model.Civilite + "', [Nomfamille]= '" + nom + "', [Prénom]= '" + prenom + "',[Tél]= '" + telph2 + "', [Fonction]= '" + fonction + "', [Email]= '" + model.mail + "' ,[Département]='" + departement + "',[Mobile]='" + telph1 + "',[Adresse1]='" + ruestage + "',[CodePostal]=" + codepostal + ",[Ville] = '" + villestage + "',[CodeTiers]='"+ model.societe + "' where RéfIndividu = " + numind + "";
                    string sql2 = "update Individus set[Civilité] = '" + model.Civilite + "', [Nomfamille]= '" + nom + "', [Prénom]= '" + prenom + "',[Tél]= '" + telph2 + "', [Fonction]= '" + fonction + "', [Email]= '" + model.mail + "' ,[Département]='" + departement + "',[Mobile]='" + telph1 + "',[Adresse1]='" + ruestage + "',[CodePostal]=" + codepostal + ",[Ville] = '" + villestage + "' where RéfIndividu = " + numind + "";

                    var obj2 = cnn.Execute(sql2);

                    string Getxml = GetXmlStringFromModel(model);
                    var GetWebcalDebut = DebutWebcal(Getxml);
                    ViewBag.resultwebcal = GetWebcalDebut;

                    //envoie nouveau webcal
                    //  string encodedData = GenerateXMLStagiaireWebCalendar(model.numindividu,model.Numero);
                    string encodedData = GenerateXMLStagiaireWebCalendarModel(model);

                    string dataXmlStagiaire = "datas=" + encodedData;
                    //planning stagiaire
                    string urlStagiaire = "http://vps676482.ovh.net/webcalendar-1.6/www/ws.php/ws/import/addStagiaire?" + Server.UrlEncode(encodedData);
                    // string retourPoste2 = SendDataToHTTP(urlStagiaire, "POST", dataXmlStagiaire);
                    string retourPoste2 = Task.Run(() => SendDataToHTTP(urlStagiaire, "POST", dataXmlStagiaire)).Result;

                    // Assuming the retourPoste2 is a JSON string, you can store it in ViewBag
                    ViewBag.JsonResponse = retourPoste2;

                    //var GetReturn = await YourAction(model);
                    ViewBag.GetReturn = YourAction(model);


                    var GetWebcalUnique = ImportNew(Getxml);
                    ViewBag.resultWebUnique = GetWebcalUnique;

                    string GetXm = GetXmlStringFromAdd(model);
                    var GetWebcalUn = ImportNewAdd(GetXm);
                    ViewBag.resultWebUn = GetWebcalUn;
                    var GetJsoText = SalleWebcal(model.numindividu, model.Numero, model.nom, model.prenom);
                    ViewBag.resultWebsall = GetJsoText;





                    List<StagiaireModel> Linguistique = StagiaireModel.Linguistique(model.Numero);
                    if (Linguistique.Count > 0)
                    {
                        var obj1 = cnn.Execute("update PROFIL set [niveausco]=" + model.niveausco + " ,[languemat]='" + langmat + "',[nbansbac]='" + nbascence + "',[premlang]='" + premierlang + "',[paysanglo]='" + sejour + "',[LangGen]=" + model.LangGen + ",[LangPro]=" + model.LangPro + ",[NationInterloc]='" + leurnational + "',[FonctInterloc]='" + leurfonct + "',[BesoinsSpecif]='" + besoins + "',[AttentesSpec]='" + attentes + "',[formatAnt]='" + formant + "',[ConfrAccueilVisite]=" + model.ConfrAccueilVisite + ",[ConfrOral]=" + model.ConfrOral + ",[ConfrPresent]=" + model.ConfrPresent + ",[ConfrNegoc]=" + model.ConfrNegoc + ",[ObjStage]='" + model.ObjStage + "',[sport]=" + model.sport + ",[jardin]=" + model.jardin + ",[musique]=" + model.musique + ",[theatre]=" + model.theatre + ",[arts]=" + model.arts + ",[sciences]=" + model.sciences + ",[litterature]=" + model.litterature + ",[bricolage]=" + model.bricolage + ",[cuisine]=" + model.cuisine + ",[autres_interets]='" + centres_interets + "' where numero =" + model.Numero + " ");

                    }
                    else
                    {
                        var obj1 = cnn.Execute("INSERT INTO PROFIL (numero,niveausco,languemat,nbansbac,premlang,paysanglo,LangGen,LangPro,NationInterloc,FonctInterloc,BesoinsSpecif,AttentesSpec,formatAnt,ConfrAccueilVisite,ConfrLecture,ConfrOral,ConfrPresent,ConfrNegoc,sport,jardin,musique,theatre,arts,sciences,litterature,bricolage,cuisine,autres_interets) VALUES (" + model.Numero + "," + model.niveausco + " ,'" + langmat + "' ,'" + nbs_absence + "','" + premierlang + "','" + sejour + "'," + model.LangGen + "," + model.LangPro + ",'" + leurnational + "','" + leurfonct + "','" + besoins + "','" + attentes + "','" + formant + "'," + model.ConfrAccueilVisite + "," + model.ConfrLecture + "," + model.ConfrOral + "," + model.ConfrPresent + "," + model.ConfrNegoc + "," + model.sport + "," + model.jardin + "," + model.musique + "," + model.theatre + "," + model.arts + "," + model.sciences + "," + model.litterature + "," + model.bricolage + "," + model.cuisine + ",'" + centres_interets + "');");

                    }
                    if (string.IsNullOrEmpty(model.languemat))
                    {
                        champvide = champvide + 1;
                    }


                    List<StagiaireModel> personnal = StagiaireModel.Personnal(model.Numero);






                    int? numindiv = 0;

                    List<StagiaireModel> data = StagiaireModel.Login(model.Numero);

                    if (data.Count > 0)
                    {
                        StagiaireModel dbUStagiaireObject = data.First();
                        Session["NumeroIndividu"] = dbUStagiaireObject.numindividu;
                        Session["civilite"] = dbUStagiaireObject.Civilite;
                        Session["fonction"] = dbUStagiaireObject.fonction;
                        Session["telephone"] = dbUStagiaireObject.Telephone;
                        Session["nom"] = dbUStagiaireObject.nom;
                        Session["departement"] = dbUStagiaireObject.Departement;
                        Session["portable"] = dbUStagiaireObject.Portable;
                        Session["prenom"] = dbUStagiaireObject.prenom;
                        Session["numero"] = dbUStagiaireObject.Numero;
                        Session["mail"] = dbUStagiaireObject.mail;
                        Session["societe"] = dbUStagiaireObject.societe;
                        Session["joursouhait"] = dbUStagiaireObject.joursouhait;
                        Session["horaisouhait"] = dbUStagiaireObject.horaisouhait;
                        Session["tempsperso"] = dbUStagiaireObject.tempsperso;
                        Session["tempsprof"] = dbUStagiaireObject.tempsprof;
                        Session["RueDuStage"] = dbUStagiaireObject.RueDuStage;
                        Session["VilleDuStage"] = dbUStagiaireObject.VilleDuStage;
                        Session["CPDuStage"] = dbUStagiaireObject.CPDuStage;
                        Session["accesnetperso"] = dbUStagiaireObject.accesnetperso;
                        Session["accesnetpro"] = dbUStagiaireObject.accesnetpro;
                        Session["e_learning"] = dbUStagiaireObject.E_learning;
                        Session["languemat"] = dbUStagiaireObject.languemat;
                        Session["niveauSco"] = dbUStagiaireObject.niveausco;
                        Session["nbansbac"] = dbUStagiaireObject.nbansbac;
                        Session["premlang"] = dbUStagiaireObject.premlang;
                        Session["paysanglo"] = dbUStagiaireObject.paysanglo;
                        Session["LangGen"] = dbUStagiaireObject.LangGen;
                        Session["LangPro"] = dbUStagiaireObject.LangPro;
                        Session["nbabscprevue"] = dbUStagiaireObject.nbabscprevue;
                        Session["arts"] = dbUStagiaireObject.arts;
                        Session["cuisine"] = dbUStagiaireObject.cuisine;
                        Session["bricolage"] = dbUStagiaireObject.bricolage;
                        Session["sport"] = dbUStagiaireObject.sport;
                        Session["sciences"] = dbUStagiaireObject.sciences;
                        Session["litterature"] = dbUStagiaireObject.litterature;
                        Session["musique"] = dbUStagiaireObject.musique;
                        Session["ConfrLecture"] = dbUStagiaireObject.ConfrLecture;
                        Session["theatre"] = dbUStagiaireObject.theatre;
                        Session["jardin"] = dbUStagiaireObject.jardin;
                        Session["autres_interets"] = dbUStagiaireObject.autres_interets;

                        Session["FormatAnt"] = dbUStagiaireObject.formatAnt;
                        Session["NationInterloc"] = dbUStagiaireObject.NationInterloc;
                        Session["FonctInterloc"] = dbUStagiaireObject.FonctInterloc;
                        Session["BesoinsSpecif"] = dbUStagiaireObject.BesoinsSpecif;
                        Session["AttentesSpec"] = dbUStagiaireObject.AttentesSpec;
                        Session["Science_Technology2"] = dbUStagiaireObject.Science_Technology2;
                        Session["Science_Technology1"] = dbUStagiaireObject.Science_Technology1;
                        Session["Parenting_Family3"] = dbUStagiaireObject.Parenting_Family3;
                        Session["Parenting_Family2"] = dbUStagiaireObject.Parenting_Family2;
                        Session["Parenting_Family1"] = dbUStagiaireObject.Parenting_Family1;
                        Session["Politics_Business7"] = dbUStagiaireObject.Politics_Business7;
                        Session["Politics_Business6"] = dbUStagiaireObject.Politics_Business6;
                        Session["Politics_Business5"] = dbUStagiaireObject.Politics_Business5;
                        Session["Politics_Business4"] = dbUStagiaireObject.Politics_Business4;
                        Session["Politics_Business3"] = dbUStagiaireObject.Politics_Business3;
                        Session["Politics_Business2"] = dbUStagiaireObject.Politics_Business2;
                        Session["Politics_Business1"] = dbUStagiaireObject.Politics_Business1;
                        Session["Nature_Outdoors3"] = dbUStagiaireObject.Nature_Outdoors3;
                        Session["Nature_Outdoors2"] = dbUStagiaireObject.Nature_Outdoors2;
                        Session["Nature_Outdoors1"] = dbUStagiaireObject.Nature_Outdoors1;
                        Session["Crafts_Hobbies6"] = dbUStagiaireObject.Crafts_Hobbies6;
                        Session["Crafts_Hobbies5"] = dbUStagiaireObject.Crafts_Hobbies5;
                        Session["Crafts_Hobbies4"] = dbUStagiaireObject.Crafts_Hobbies4;
                        Session["Crafts_Hobbies3"] = dbUStagiaireObject.Crafts_Hobbies3;
                        Session["Crafts_Hobbies2"] = dbUStagiaireObject.Crafts_Hobbies2;
                        Session["Crafts_Hobbies1"] = dbUStagiaireObject.Crafts_Hobbies1;
                        Session["Health_Fitness_Sport5"] = dbUStagiaireObject.Health_Fitness_Sport5;
                        Session["Health_Fitness_Sport4"] = dbUStagiaireObject.Health_Fitness_Sport4;
                        Session["Health_Fitness_Sport3"] = dbUStagiaireObject.Health_Fitness_Sport3;
                        Session["Health_Fitness_Sport2"] = dbUStagiaireObject.Health_Fitness_Sport2;
                        Session["Health_Fitness_Sport1"] = dbUStagiaireObject.Health_Fitness_Sport1;
                        Session["Travelling_Lifestyle10"] = dbUStagiaireObject.Travelling_Lifestyle10;
                        Session["Travelling_Lifestyle9"] = dbUStagiaireObject.Travelling_Lifestyle9;
                        Session["Travelling_Lifestyle8"] = dbUStagiaireObject.Travelling_Lifestyle8;
                        Session["Travelling_Lifestyle7"] = dbUStagiaireObject.Travelling_Lifestyle7;
                        Session["Travelling_Lifestyle6"] = dbUStagiaireObject.Travelling_Lifestyle6;
                        Session["Travelling_Lifestyle5"] = dbUStagiaireObject.Travelling_Lifestyle5;
                        Session["Travelling_Lifestyle4"] = dbUStagiaireObject.Travelling_Lifestyle4;
                        Session["Travelling_Lifestyle3"] = dbUStagiaireObject.Travelling_Lifestyle3;
                        Session["Travelling_Lifestyle2"] = dbUStagiaireObject.Travelling_Lifestyle2;
                        Session["Travelling_Lifestyle1"] = dbUStagiaireObject.Travelling_Lifestyle1;
                        Session["Cinema_Arts13"] = dbUStagiaireObject.Cinema_Arts13;
                        Session["Cinema_Arts12"] = dbUStagiaireObject.Cinema_Arts12;
                        Session["Cinema_Arts11"] = dbUStagiaireObject.Cinema_Arts11;
                        Session["Cinema_Arts10"] = dbUStagiaireObject.Cinema_Arts10;
                        Session["Cinema_Arts9"] = dbUStagiaireObject.Cinema_Arts9;
                        Session["Cinema_Arts8"] = dbUStagiaireObject.Cinema_Arts8;
                        Session["Cinema_Arts7"] = dbUStagiaireObject.Cinema_Arts7;
                        Session["Cinema_Arts6"] = dbUStagiaireObject.Cinema_Arts6;
                        Session["Cinema_Arts5"] = dbUStagiaireObject.Cinema_Arts5;
                        Session["Cinema_Arts4"] = dbUStagiaireObject.Cinema_Arts4;
                        Session["Cinema_Arts3"] = dbUStagiaireObject.Cinema_Arts3;
                        Session["Cinema_Arts2"] = dbUStagiaireObject.Cinema_Arts2;
                        Session["Cinema_Arts1"] = dbUStagiaireObject.Cinema_Arts1;
                        numindiv = dbUStagiaireObject.numindividu;


                        var listeDesSocietes = StagiaireModel.GetAllSocietes();

                        // Assigner la liste des sociétés au modèle
                        model.ListeDesSocietes = listeDesSocietes;
                    }
                    string t = TokenManager.GenerateToken(model.Numero);

                    /* if (numindiv != 0)
                     {
                         ViewBag.Numeroindividu = numindiv;  // var obj3 = cnn.Execute("update stagiaires_profil")
                     }
                     else
                     {
                         var numeroindividu = numeroIndividu();
                         var obj4 = cnn.Execute("INSERT INTO Individus(CodeTiers,Civilité,Nomfamille,Prénom,Fonction,Email,Adresse1,Ville,Tél)VALUES('" + model.Societe + "','" + model.Civilite + "','" + model.Nom + "','" + model.Prenom + "','" + model.Fonction + "','" + model.Mail + "','" + ruestage + "','" + villestage + "','" + model.Telephone + "')");

                         var obj5 = cnn.Execute("update Stagiaire_profil set [numindividu]=" + numeroindividu + " where numero =" + model.Numero + " ");
                     }*/
                    ViewBag.Champ = champvide;
                    //  return RedirectToAction("Edit", "Home");
                    if (champvide > 0)
                    {
                        ViewBag.ResMessege = ResMessege.getDanger("Veuiller remplir les champs vide s'il vous plaît");
                        return View(model);
                    }
                    else
                    {

                        /*   BindDatalist(model);
                            string xml = "";
                            if (Request.InputStream != null)
                            {
                                StreamReader stream = new StreamReader(Request.InputStream);
                                string x = stream.ReadToEnd();
                                xml = HttpUtility.UrlDecode(x);
                            }*/
                        string urlwebcal = "https://formateur.forma2plus.com/auto.php/webservices/security/authenticate?";
                        string token = GetTokenFromUrl(model.numindividu); // Supposons que cette fonction retourne le jeton
                        string urlWithToken = $"{urlwebcal}token={token}";
                        ViewBag.urlwebcal = urlWithToken;
                        ViewBag.idInd = model.numindividu;
                        var xml = GetPHPXML(model);
                        ViewBag.Reponse = xml;

                        string datecrea = model.nom;
                        if (model.nom != null)
                        {
                            datecrea = model.nom.Substring(0, 2); // Récupérer les deux premières lettres
                            datecrea = datecrea.ToUpper();
                        }
                        if (model.nom_change)
                        {

                            try
                            {

                                var senderemail = new MailAddress("suividemonstage@forma2plus.com", "FORMA 2+");
                                var receiveremail = new MailAddress(model.mail, "Receiver");
                                 // var receiveremail = new MailAddress("hermone@forma2plus.com", "Receiver");
                              //  var ccemail = new MailAddress("helene@forma2plus.com", "Helene");
                                var bccemail = new MailAddress("tessie@forma2plus.com", "Tessie");

                                var password = "hermone";
                                string subject = "FORMA 2+ Mise à jour de votre fiche profil signalétique";
                                //  string body = "Bonjour,<br/><br/> Votre profil a été mis à jour, et nous voulions nous assurer que vous avez accès aux informations les plus récentes.<br/><br/>Pour accéder à votre profil mis à jour, veuillez utiliser le lien suivant: https://extranet.forma2plus.com/Profil/Plogon/" + model.Numero+"/"+nom+ ".<br/><br/>Votre Identifiant: " + nom+ "<br/>Votre numéro de dossier : " + model.Numero+ ".<br/><br/>N\'hésitez pas à nous contacter si vous avez des questions ou des préoccupations concernant ces modifications.<br/><br/> Cordialement ";
                                string body = "Bonjour,\n\nVotre profil a été mis à jour, et nous voulions nous assurer que vous avez accès aux informations les plus récentes.\nPour accéder à votre profil mis à jour, veuillez utiliser le lien suivant : https://extranet.forma2plus.com/Profil/index \nVotre Identifiant: " + model.mail + "\nVotre numéro de dossier : " + model.Numero + "\n\nN'hésitez pas à nous contacter si vous avez des questions.\n\nCordialement \n\nEquipe FORMA 2+ ";

                                var smtp = new SmtpClient
                                {
                                    Host = "mail.forma2plus.com",
                                    Port = 25,
                                    EnableSsl = true,
                                    DeliveryMethod = SmtpDeliveryMethod.Network,
                                    UseDefaultCredentials = false,
                                    Credentials = new NetworkCredential(senderemail.Address, password)

                                };
                                using (var mes = new MailMessage(senderemail, receiveremail)
                                {
                                    Subject = subject,
                                    Body = body,

                                })
                                {
                                    // mes.CC.Add(ccemail); // Ajout de la copie à Helene
                                    mes.Bcc.Add(bccemail); // Ajout de la copie cachée à Helene

                                    smtp.Send(mes);
                                }
                            }

                            catch (Exception ex)
                            {
                                // Gérer l'erreur ici
                                // Vous pouvez journaliser l'erreur, afficher un message à l'utilisateur, etc.
                                // Rediriger l'utilisateur vers une page d'erreur
                                return RedirectToAction("Logins", "Home", new { t });
                            }
                        }

                    }
                    //   string t = TokenManager.GenerateToken(model.Numero);

                    return RedirectToAction("Logins", "Home", new { t });


                }

                return View(model);
            }
            catch (Exception ex)
            {
                // Gérer l'erreur ici
                // Vous pouvez journaliser l'erreur, afficher un message à l'utilisateur, etc.
                // Rediriger l'utilisateur vers une page d'erreur
                return RedirectToAction("Notfound");
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Editprofil(StagiaireModel model)
        {
            //  var obj = cnn.Execute("update STAGIAIRE set [fonction]='" + model.Fonction + "', [mail]='" + model.Mail + "',[civilite]='" + model.Civilite + "',[telephone]='" + model.Telephone + "',[nom]='" + model.Nom + "',[prenom]='" + model.Prenom + "',[departement]='" + model.Departement + "',[portable]='" + model.Portable + "',[societe]='" + model.Societe + "',[joursouhait]='" + model.Joursouhait + "',[tempsprof]='" + model.Tempsprof + "',[RueDuStage]='" + model.RueDuStage + "',[VilleDuStage]='" + model.VilleDuStage + "',[CPDuStage]='" + model.CPDuStage + "',[accesnetpro]=" + model.Accesnetpro + ",[horaisouhait]='" + model.Horaisouhait + "',[e_learning]=" + model.E_learning + ",[accesnetperso]=" + model.Accesnetperso + ",[nbabscprevue]='" + model.Nbabscprevue + "'  where numero =" + model.Numero + " ");
            //  var obj1 = cnn.Execute("update PROFIL set [niveausco]=" + model.NiveauSco + " ,[languemat]='" + model.Languemat + "',[nbansbac]='" + model.Nbansbac + "',[premlang]='" + model.Premlang + "',[paysanglo]='" + model.paysanglo + "',[LangGen]=" + model.LangGen + ",[LangPro]=" + model.LangPro+ ",[NationInterloc]='" + model.NationInterloc + "',[FonctInterloc]='" + model.FonctInterloc + "',[BesoinsSpecif]='" + model.BesoinsSpecif + "',[AttentesSpec]='" + model.AttentesSpec + "',[formatAnt]='" + model.FormatAnt + "',[ConfrAccueilVisite]=" + model.ConfrAccueilVisite + ",[ConfrLecture]=" + model.ConfrLecture + ",[ConfrOral]=" + model.ConfrOral + ",[ConfrPresent]=" + model.ConfrPresent + ",[ConfrNegoc]=" + model.ConfrNegoc + ",[ObjStage]='" + model.ObjStage + "',[sport]=" + model.Sport + ",[jardin]=" + model.Jardin + ",[musique]=" + model.Musique + ",[theatre]=" + model.Theatre + ",[arts]=" + model.Arts + ",[sciences]=" + model.Sciences + ",[litterature]=" + model.Litterature + ",[bricolage]=" + model.Bricolage + ",[cuisine]=" + model.Cuisine + ",[autres_interets]='" + model.Autres_interets + "' where numero =" + model.Numero + " ");
            //  StagiaireModel model = new StagiaireModel();
           if (ModelState.IsValid)
            {

                string attentes, besoins, villestage, ruestage, centres_interets, nom, prenom,societe, nbascence, fonction, departement, codepostal,langmat,premierlang,sejour, leurfonct, leurnational,domaine,  autres;

                fonction = conversion(model.fonction);
                departement = conversion(model.Departement);
                codepostal = conversion(model.CPDuStage);
                langmat = conversion(model.languemat);
                premierlang = conversion(model.premlang);
                sejour = conversion(model.paysanglo);
                leurfonct = conversion(model.FonctInterloc);
                leurnational = conversion(model.NationInterloc);
                domaine = conversion(model.AttentesSpec);
                autres = conversion(model.autres_interets);
                nom = WebUtility.HtmlDecode( model.nom);
                if (!(string.IsNullOrEmpty(nom)))
                {
                    nom = nom.Replace("'", "''");
                }
                if ((string.IsNullOrEmpty(fonction)))
                {
                    fonction = ".";
                }
                nbascence = WebUtility.HtmlDecode(model.nbansbac);
                if (!(string.IsNullOrEmpty(nbascence)))
                {
                    nbascence = nbascence.Replace("'", "''");
                }
                societe = WebUtility.HtmlDecode(model.societe);
                if (!(string.IsNullOrEmpty(societe)))
                {
                    societe = societe.Replace("'", "''");
                }
                prenom = WebUtility.HtmlDecode(model.prenom);
                if (!(string.IsNullOrEmpty(prenom)))
                {
                    prenom = prenom.Replace("'", "''");
                }
                attentes = conversion(( model.AttentesSpec));
                if ((string.IsNullOrEmpty(attentes)))
                {
                    attentes = ".";
                }
                besoins = conversion( model.BesoinsSpecif);
               // besoins = WebUtility.HtmlDecode(model.AttentesSpec);
                if ((string.IsNullOrEmpty(besoins)))
                {
                    besoins = ".";
                }
                villestage = conversion(model.VilleDuStage);
                

                ruestage = conversion( model.RueDuStage);
                if (!(string.IsNullOrEmpty(ruestage)))
                {
                    ruestage = ruestage.Replace("'", "''");
                    ruestage = ruestage.Replace("&", "");
                    ruestage = ruestage.Replace("#", "");
                    ruestage = ruestage.Replace(";", "");

                }
                
                centres_interets = conversion(model.autres_interets);
                if (!(string.IsNullOrEmpty(centres_interets)))
                {
                    centres_interets = centres_interets.Replace("'", "''");
                }
                else
                {
                    centres_interets = ".";
                }
                var champvide = 0;
                var telph1 = model.Portable;
                if (string.IsNullOrEmpty(model.Portable))
                {
                  telph1 = conversion(model.Portable);
                    champvide = 1;
                }
                
                var telph2 = model.Telephone;

                if (string.IsNullOrEmpty(telph2) || telph2 == ".")
                {
                    telph2 = telph1;
                }

               var temps_prof =conversion( model.tempsprof);
                
                var temps_perso = conversion(model.tempsperso);
                
                var jour_essa = conversion(model.joursouhait);
               
            
                var nbs_absencpreve = conversion( model.nbabscprevue);
                
                var nbs_absence = conversion(model.nbansbac);
              
                var formant = conversion(model.formatAnt);
                var horairesouhait = conversion(model.horaisouhait);
                var numind = model.numindividu;
                

                // Check if model.numindividu is nequal to 0 and not null

            /*    string obtest = "SELECT stagiaire_profil.numero, stagiaire_profil.nom, stagiaire_profil.numindividu FROM stagiaire_profil WHERE(((stagiaire_profil.numero) = "+model.Numero+"))";
                var objs = cnn.Query<StagiaireModel>(obtest);
                if (objs != null)
                {
                    StagiaireModel nombre = new StagiaireModel();

                    nombre.numindividu = objs.FirstOrDefault().numindividu;
                   
                }*/
               if (codepostal =="." )
                {
                    codepostal = "0";
                }
                var obj = cnn.Execute("update stagiaire_profil set [fonction]='" + fonction + "', [mail]='" + model.mail + "',[civilite]='" + model.Civilite + "',[telephone]='" + telph2+ "',[nom]='" + nom + "',[prenom]='" + prenom+ "',[departement]='" + departement + "',[portable]='" + telph1 + "',[joursouhait]='" + jour_essa + "',[tempsprof]='" + temps_prof + "',[tempsperso]='" + temps_perso + "',[RueDuStage]='" + ruestage + "',[VilleDuStage]='" + villestage + "',[CPDuStage]='" + codepostal + "',[accesnetpro]=" + model.accesnetpro + ",[horaisouhait]='" + horairesouhait + "',[e_learning]=" + model.E_learning + ",[accesnetperso]=" + model.accesnetperso + ",[nbabscprevue]='" + nbs_absencpreve + "', [societe]='"+ model.societe +"'  where numero =" + model.Numero + " ");
              //  string sql2 = "update Individus set[Civilité] = '" + model.Civilite + "', [Nomfamille]= '" + nom + "', [Prénom]= '" + prenom + "',[Tél]= '" + telph2 + "', [Fonction]= '" + fonction + "', [Email]= '" + model.mail + "' ,[Département]='" + departement + "',[Mobile]='" + telph1 + "',[Adresse1]='" + ruestage + "',[CodePostal]=" + codepostal + ",[Ville] = '" + villestage + "',[CodeTiers]='"+ model.societe + "' where RéfIndividu = " + numind + "";
                string sql2 = "update Individus set[Civilité] = '" + model.Civilite + "', [Nomfamille]= '" + nom + "', [Prénom]= '" + prenom + "',[Tél]= '" + telph2 + "', [Fonction]= '" + fonction + "', [Email]= '" + model.mail + "' ,[Département]='" + departement + "',[Mobile]='" + telph1 + "',[Adresse1]='" + ruestage + "',[CodePostal]=" + codepostal + ",[Ville] = '" + villestage + "' where RéfIndividu = " + numind + "";

                var obj2 = cnn.Execute(sql2);

               string Getxml = GetXmlStringFromModel(model);
               var GetWebcalDebut = DebutWebcal(Getxml);
                ViewBag.resultwebcal = GetWebcalDebut;

                //envoie nouveau webcal
              //  string encodedData = GenerateXMLStagiaireWebCalendar(model.numindividu,model.Numero);
                string encodedData = GenerateXMLStagiaireWebCalendarModel(model);

                string dataXmlStagiaire = "datas=" + encodedData;
                //planning stagiaire
                string urlStagiaire = "http://vps676482.ovh.net/webcalendar-1.6/www/ws.php/ws/import/addStagiaire?" + Server.UrlEncode(encodedData);
                // string retourPoste2 = SendDataToHTTP(urlStagiaire, "POST", dataXmlStagiaire);
                string retourPoste2 = Task.Run(() => SendDataToHTTP(urlStagiaire, "POST", dataXmlStagiaire)).Result;

                // Assuming the retourPoste2 is a JSON string, you can store it in ViewBag
                ViewBag.JsonResponse = retourPoste2;

                //var GetReturn = await YourAction(model);
                ViewBag.GetReturn = YourAction(model);
               

                var GetWebcalUnique = ImportNew(Getxml);
                ViewBag.resultWebUnique = GetWebcalUnique;

                string GetXm = GetXmlStringFromAdd(model);
                var GetWebcalUn = ImportNewAdd(GetXm);
                ViewBag.resultWebUn = GetWebcalUn;
                var GetJsoText = SalleWebcal(model.numindividu, model.Numero, model.nom, model.prenom);
                ViewBag.resultWebsall = GetJsoText;

                
                           
                        

                        List<StagiaireModel> Linguistique = StagiaireModel.Linguistique(model.Numero);
                if (Linguistique.Count > 0)
                {
                    var obj1 = cnn.Execute("update PROFIL set [niveausco]=" + model.niveausco + " ,[languemat]='" +langmat+ "',[nbansbac]='" + nbascence+ "',[premlang]='" + premierlang + "',[paysanglo]='" + sejour + "',[LangGen]=" + model.LangGen + ",[LangPro]=" + model.LangPro + ",[NationInterloc]='" + leurnational + "',[FonctInterloc]='" + leurfonct + "',[BesoinsSpecif]='" + besoins + "',[AttentesSpec]='" + attentes + "',[formatAnt]='" + formant + "',[ConfrAccueilVisite]=" + model.ConfrAccueilVisite + ",[ConfrOral]=" + model.ConfrOral + ",[ConfrPresent]=" + model.ConfrPresent + ",[ConfrNegoc]=" + model.ConfrNegoc + ",[ObjStage]='" + model.ObjStage + "',[sport]=" + model.sport + ",[jardin]=" + model.jardin + ",[musique]=" + model.musique + ",[theatre]=" + model.theatre + ",[arts]=" + model.arts + ",[sciences]=" + model.sciences + ",[litterature]=" + model.litterature + ",[bricolage]=" + model.bricolage + ",[cuisine]=" + model.cuisine + ",[autres_interets]='" + centres_interets + "' where numero =" + model.Numero + " ");
                   
                }
                else
                {
                    var obj1 = cnn.Execute("INSERT INTO PROFIL (numero,niveausco,languemat,nbansbac,premlang,paysanglo,LangGen,LangPro,NationInterloc,FonctInterloc,BesoinsSpecif,AttentesSpec,formatAnt,ConfrAccueilVisite,ConfrLecture,ConfrOral,ConfrPresent,ConfrNegoc,sport,jardin,musique,theatre,arts,sciences,litterature,bricolage,cuisine,autres_interets) VALUES (" + model.Numero + "," + model.niveausco + " ,'" +langmat + "' ,'" +nbs_absence + "','" + premierlang + "','" + sejour + "'," + model.LangGen + "," + model.LangPro + ",'" + leurnational + "','" + leurfonct + "','" + besoins + "','" + attentes + "','" + formant + "'," + model.ConfrAccueilVisite + "," + model.ConfrLecture + "," + model.ConfrOral + "," + model.ConfrPresent + "," + model.ConfrNegoc + "," + model.sport + "," + model.jardin + "," + model.musique + "," + model.theatre + "," + model.arts + "," + model.sciences + "," + model.litterature + "," + model.bricolage + "," + model.cuisine + ",'" + centres_interets + "');");

                }
                if (string.IsNullOrEmpty(model.languemat))
                {
                    champvide = champvide + 1;
                }


                List<StagiaireModel> personnal = StagiaireModel.Personnal(model.Numero);



              /*  if (personnal.Count > 0)
                {
                    var obj2 = cnn.Execute("update  personal_interest set [date_maj]=now() ,[Cinema_Arts1]=" + model.Cinema_Arts1 + ",[Cinema_Arts2]=" + model.Cinema_Arts2 + ", [Cinema_Arts3]=" + model.Cinema_Arts3 + ",[Cinema_Arts4]=" + model.Cinema_Arts4 + ",[Cinema_Arts5]=" + model.Cinema_Arts5 + ",[Cinema_Arts6]=" + model.Cinema_Arts6 + ",[Cinema_Arts7]=" + model.Cinema_Arts7 + ",[Cinema_Arts8]=" + model.Cinema_Arts8 + ",[Cinema_Arts9]=" + model.Cinema_Arts9 + ",[Cinema_Arts10]=" + model.Cinema_Arts10 + ",[Cinema_Arts11]=" + model.Cinema_Arts11 + ",[Cinema_Arts12]=" + model.Cinema_Arts12 + ",[Cinema_Arts13]=" + model.Cinema_Arts13 + ",[Health_Fitness_Sport1]=" + model.Health_Fitness_Sport1 + ",[Health_Fitness_Sport2]=" + model.Health_Fitness_Sport2 + ",[Health_Fitness_Sport3]=" + model.Health_Fitness_Sport3 + ",[Health_Fitness_Sport4]=" + model.Health_Fitness_Sport4 + ",[Health_Fitness_Sport5]=" + model.Health_Fitness_Sport5 + ",[Crafts_Hobbies1]=" + model.Crafts_Hobbies1 + ",[Crafts_Hobbies2]=" + model.Crafts_Hobbies2 + ",[Crafts_Hobbies3]=" + model.Crafts_Hobbies3 + ",[Crafts_Hobbies4]=" + model.Crafts_Hobbies4 + ",[Crafts_Hobbies5]=" + model.Crafts_Hobbies6 + ",[Nature_Outdoors1]=" + model.Nature_Outdoors1 + ",[Nature_Outdoors2]=" + model.Nature_Outdoors2 + ",[Nature_Outdoors3]=" + model.Nature_Outdoors3 + ",[Travelling_Lifestyle1]=" + model.Travelling_Lifestyle1 + ",[Travelling_Lifestyle2]=" + model.Travelling_Lifestyle2 + ",[Travelling_Lifestyle3]=" + model.Travelling_Lifestyle3 + ",[Travelling_Lifestyle4]=" + model.Travelling_Lifestyle4 + ",[Travelling_Lifestyle5]=" + model.Travelling_Lifestyle5 + ",[Travelling_Lifestyle6]=" + model.Travelling_Lifestyle6 + ",[Travelling_Lifestyle7]=" + model.Travelling_Lifestyle7 + ",[Travelling_Lifestyle8]=" + model.Travelling_Lifestyle8 + ",[Travelling_Lifestyle9]=" + model.Travelling_Lifestyle9 + ",[Travelling_Lifestyle10]=" + model.Travelling_Lifestyle10 + ",[Politics_Business1]=" + model.Politics_Business1 + ",[Politics_Business2]=" + model.Politics_Business2 + ",[Politics_Business3]=" + model.Politics_Business3 + ",[Politics_Business4]=" + model.Politics_Business4 + ",[Politics_Business5]=" + model.Politics_Business5 + ",[Politics_Business6]=" + model.Politics_Business6 + ", [Politics_Business7]=" + model.Politics_Business7 + ",[Parenting_Family1]=" + model.Parenting_Family1 + ",[Parenting_Family2]=" + model.Parenting_Family2 + ",[Parenting_Family3]=" + model.Parenting_Family3 + ",[Science_Technology1]=" + model.Science_Technology1 + ",[Science_Technology2]=" + model.Science_Technology2 + " where personal_interest.numero_profil =" + model.Numero + "");


                }
                else
                {
                    var obj2 = cnn.Execute("INSERT INTO personal_interest (numero_profil, date_maj, Cinema_Arts1, Cinema_Arts2, Cinema_Arts3,Cinema_Arts4,Cinema_Arts5,Cinema_Arts6,Cinema_Arts7,Cinema_Arts8,Cinema_Arts9,Cinema_Arts10,Cinema_Arts11,Cinema_Arts12,Cinema_Arts13,Health_Fitness_Sport1, Health_Fitness_Sport2,Health_Fitness_Sport3,Health_Fitness_Sport4,Health_Fitness_Sport5,Crafts_Hobbies1,Crafts_Hobbies5,Crafts_Hobbies2,Crafts_Hobbies3,Crafts_Hobbies4,Crafts_Hobbies6,Nature_Outdoors1,Nature_Outdoors2,Nature_Outdoors3,Travelling_Lifestyle1,Travelling_Lifestyle2,Travelling_Lifestyle3,Travelling_Lifestyle4,Travelling_Lifestyle5,Travelling_Lifestyle6,Travelling_Lifestyle7,Travelling_Lifestyle8,Travelling_Lifestyle9,Travelling_Lifestyle10,Politics_Business1,Politics_Business2,Politics_Business3,Politics_Business4,Politics_Business5,Politics_Business6,Politics_Business7 , Parenting_Family1, Parenting_Family2, Parenting_Family3, Science_Technology1, Science_Technology2) VALUES(" + model.Numero + ",now() ," + model.Cinema_Arts1 + "," + model.Cinema_Arts2 + "," + model.Cinema_Arts3 + "," + model.Cinema_Arts4 + "," + model.Cinema_Arts5 + "," + model.Cinema_Arts6 + "," + model.Cinema_Arts7 + "," + model.Cinema_Arts8 + "," + model.Cinema_Arts9 + "," + model.Cinema_Arts10 + "," + model.Cinema_Arts11 + "," + model.Cinema_Arts12 + "," + model.Cinema_Arts13 + "," + model.Health_Fitness_Sport1 + "," + model.Health_Fitness_Sport2 + "," + model.Health_Fitness_Sport3 + "," + model.Health_Fitness_Sport4 + "," + model.Health_Fitness_Sport5 + "," + model.Crafts_Hobbies1 + "," + model.Crafts_Hobbies5 + "," + model.Crafts_Hobbies2 + "," + model.Crafts_Hobbies3 + "," + model.Crafts_Hobbies4 + "," + model.Crafts_Hobbies6 + "," + model.Nature_Outdoors1 + "," + model.Nature_Outdoors2 + "," + model.Nature_Outdoors3 + "," + model.Travelling_Lifestyle1 + "," + model.Travelling_Lifestyle2 + "," + model.Travelling_Lifestyle3 + "," + model.Travelling_Lifestyle4 + "," + model.Travelling_Lifestyle5 + "," + model.Travelling_Lifestyle6 + "," + model.Travelling_Lifestyle7 + "," + model.Travelling_Lifestyle8 + "," + model.Travelling_Lifestyle9 + "," + model.Travelling_Lifestyle10 + "," + model.Politics_Business1 + "," + model.Politics_Business2 + "," + model.Politics_Business3 + "," + model.Politics_Business4 + "," + model.Politics_Business5 + "," + model.Politics_Business6 + ", " + model.Politics_Business7 + "," + model.Parenting_Family1 + "," + model.Parenting_Family2 + "," + model.Parenting_Family3 + "," + model.Science_Technology1 + "," + model.Science_Technology2 + ")");

                }
                if (string.IsNullOrEmpty(model.Autres_interets))
                {
                    //  champvide = 1;
                    var interes = 0;
                }*/



                int? numindiv = 0;

                List<StagiaireModel> data = StagiaireModel.Login(model.Numero);

                if (data.Count > 0)
                {
                    StagiaireModel dbUStagiaireObject = data.First();
                    Session["NumeroIndividu"] = dbUStagiaireObject.numindividu;
                    Session["civilite"] = dbUStagiaireObject.Civilite;
                    Session["fonction"] = dbUStagiaireObject.fonction;
                    Session["telephone"] = dbUStagiaireObject.Telephone;
                    Session["nom"] = dbUStagiaireObject.nom;
                    Session["departement"] = dbUStagiaireObject.Departement;
                    Session["portable"] = dbUStagiaireObject.Portable;
                    Session["prenom"] = dbUStagiaireObject.prenom;
                    Session["numero"] = dbUStagiaireObject.Numero;
                    Session["mail"] = dbUStagiaireObject.mail;
                    Session["societe"] = dbUStagiaireObject.societe;
                    Session["joursouhait"] = dbUStagiaireObject.joursouhait;
                    Session["horaisouhait"] = dbUStagiaireObject.horaisouhait;
                    Session["tempsperso"] = dbUStagiaireObject.tempsperso;
                    Session["tempsprof"] = dbUStagiaireObject.tempsprof;
                    Session["RueDuStage"] = dbUStagiaireObject.RueDuStage;
                    Session["VilleDuStage"] = dbUStagiaireObject.VilleDuStage;
                    Session["CPDuStage"] = dbUStagiaireObject.CPDuStage;
                    Session["accesnetperso"] = dbUStagiaireObject.accesnetperso;
                    Session["accesnetpro"] = dbUStagiaireObject.accesnetpro;
                    Session["e_learning"] = dbUStagiaireObject.E_learning;
                    Session["languemat"] = dbUStagiaireObject.languemat;
                    Session["niveauSco"] = dbUStagiaireObject.niveausco;
                    Session["nbansbac"] = dbUStagiaireObject.nbansbac;
                    Session["premlang"] = dbUStagiaireObject.premlang;
                    Session["paysanglo"] = dbUStagiaireObject.paysanglo;
                    Session["LangGen"] = dbUStagiaireObject.LangGen;
                    Session["LangPro"] = dbUStagiaireObject.LangPro;
                    Session["nbabscprevue"] = dbUStagiaireObject.nbabscprevue;
                    Session["arts"] = dbUStagiaireObject.arts;
                    Session["cuisine"] = dbUStagiaireObject.cuisine;
                    Session["bricolage"] = dbUStagiaireObject.bricolage;
                    Session["sport"] = dbUStagiaireObject.sport;
                    Session["sciences"] = dbUStagiaireObject.sciences;
                    Session["litterature"] = dbUStagiaireObject.litterature;
                    Session["musique"] = dbUStagiaireObject.musique;
                    Session["ConfrLecture"] = dbUStagiaireObject.ConfrLecture;
                    Session["theatre"] = dbUStagiaireObject.theatre;
                    Session["jardin"] = dbUStagiaireObject.jardin;
                    Session["autres_interets"] = dbUStagiaireObject.autres_interets;

                    Session["FormatAnt"] = dbUStagiaireObject.formatAnt;
                    Session["NationInterloc"] = dbUStagiaireObject.NationInterloc;
                    Session["FonctInterloc"] = dbUStagiaireObject.FonctInterloc;
                    Session["BesoinsSpecif"] = dbUStagiaireObject.BesoinsSpecif;
                    Session["AttentesSpec"] = dbUStagiaireObject.AttentesSpec;
                    Session["Science_Technology2"] = dbUStagiaireObject.Science_Technology2;
                    Session["Science_Technology1"] = dbUStagiaireObject.Science_Technology1;
                    Session["Parenting_Family3"] = dbUStagiaireObject.Parenting_Family3;
                    Session["Parenting_Family2"] = dbUStagiaireObject.Parenting_Family2;
                    Session["Parenting_Family1"] = dbUStagiaireObject.Parenting_Family1;
                    Session["Politics_Business7"] = dbUStagiaireObject.Politics_Business7;
                    Session["Politics_Business6"] = dbUStagiaireObject.Politics_Business6;
                    Session["Politics_Business5"] = dbUStagiaireObject.Politics_Business5;
                    Session["Politics_Business4"] = dbUStagiaireObject.Politics_Business4;
                    Session["Politics_Business3"] = dbUStagiaireObject.Politics_Business3;
                    Session["Politics_Business2"] = dbUStagiaireObject.Politics_Business2;
                    Session["Politics_Business1"] = dbUStagiaireObject.Politics_Business1;
                    Session["Nature_Outdoors3"] = dbUStagiaireObject.Nature_Outdoors3;
                    Session["Nature_Outdoors2"] = dbUStagiaireObject.Nature_Outdoors2;
                    Session["Nature_Outdoors1"] = dbUStagiaireObject.Nature_Outdoors1;
                    Session["Crafts_Hobbies6"] = dbUStagiaireObject.Crafts_Hobbies6;
                    Session["Crafts_Hobbies5"] = dbUStagiaireObject.Crafts_Hobbies5;
                    Session["Crafts_Hobbies4"] = dbUStagiaireObject.Crafts_Hobbies4;
                    Session["Crafts_Hobbies3"] = dbUStagiaireObject.Crafts_Hobbies3;
                    Session["Crafts_Hobbies2"] = dbUStagiaireObject.Crafts_Hobbies2;
                    Session["Crafts_Hobbies1"] = dbUStagiaireObject.Crafts_Hobbies1;
                    Session["Health_Fitness_Sport5"] = dbUStagiaireObject.Health_Fitness_Sport5;
                    Session["Health_Fitness_Sport4"] = dbUStagiaireObject.Health_Fitness_Sport4;
                    Session["Health_Fitness_Sport3"] = dbUStagiaireObject.Health_Fitness_Sport3;
                    Session["Health_Fitness_Sport2"] = dbUStagiaireObject.Health_Fitness_Sport2;
                    Session["Health_Fitness_Sport1"] = dbUStagiaireObject.Health_Fitness_Sport1;
                    Session["Travelling_Lifestyle10"] = dbUStagiaireObject.Travelling_Lifestyle10;
                    Session["Travelling_Lifestyle9"] = dbUStagiaireObject.Travelling_Lifestyle9;
                    Session["Travelling_Lifestyle8"] = dbUStagiaireObject.Travelling_Lifestyle8;
                    Session["Travelling_Lifestyle7"] = dbUStagiaireObject.Travelling_Lifestyle7;
                    Session["Travelling_Lifestyle6"] = dbUStagiaireObject.Travelling_Lifestyle6;
                    Session["Travelling_Lifestyle5"] = dbUStagiaireObject.Travelling_Lifestyle5;
                    Session["Travelling_Lifestyle4"] = dbUStagiaireObject.Travelling_Lifestyle4;
                    Session["Travelling_Lifestyle3"] = dbUStagiaireObject.Travelling_Lifestyle3;
                    Session["Travelling_Lifestyle2"] = dbUStagiaireObject.Travelling_Lifestyle2;
                    Session["Travelling_Lifestyle1"] = dbUStagiaireObject.Travelling_Lifestyle1;
                    Session["Cinema_Arts13"] = dbUStagiaireObject.Cinema_Arts13;
                    Session["Cinema_Arts12"] = dbUStagiaireObject.Cinema_Arts12;
                    Session["Cinema_Arts11"] = dbUStagiaireObject.Cinema_Arts11;
                    Session["Cinema_Arts10"] = dbUStagiaireObject.Cinema_Arts10;
                    Session["Cinema_Arts9"] = dbUStagiaireObject.Cinema_Arts9;
                    Session["Cinema_Arts8"] = dbUStagiaireObject.Cinema_Arts8;
                    Session["Cinema_Arts7"] = dbUStagiaireObject.Cinema_Arts7;
                    Session["Cinema_Arts6"] = dbUStagiaireObject.Cinema_Arts6;
                    Session["Cinema_Arts5"] = dbUStagiaireObject.Cinema_Arts5;
                    Session["Cinema_Arts4"] = dbUStagiaireObject.Cinema_Arts4;
                    Session["Cinema_Arts3"] = dbUStagiaireObject.Cinema_Arts3;
                    Session["Cinema_Arts2"] = dbUStagiaireObject.Cinema_Arts2;
                    Session["Cinema_Arts1"] = dbUStagiaireObject.Cinema_Arts1;
                    numindiv = dbUStagiaireObject.numindividu;


                    var listeDesSocietes = StagiaireModel.GetAllSocietes();

                    // Assigner la liste des sociétés au modèle
                    model.ListeDesSocietes = listeDesSocietes;
                }
                /* if (numindiv != 0)
                 {
                     ViewBag.Numeroindividu = numindiv;  // var obj3 = cnn.Execute("update stagiaires_profil")
                 }
                 else
                 {
                     var numeroindividu = numeroIndividu();
                     var obj4 = cnn.Execute("INSERT INTO Individus(CodeTiers,Civilité,Nomfamille,Prénom,Fonction,Email,Adresse1,Ville,Tél)VALUES('" + model.Societe + "','" + model.Civilite + "','" + model.Nom + "','" + model.Prenom + "','" + model.Fonction + "','" + model.Mail + "','" + ruestage + "','" + villestage + "','" + model.Telephone + "')");

                     var obj5 = cnn.Execute("update Stagiaire_profil set [numindividu]=" + numeroindividu + " where numero =" + model.Numero + " ");
                 }*/
                ViewBag.Champ = champvide;
                //  return RedirectToAction("Edit", "Home");
                if (champvide > 0)
                {
                    ViewBag.ResMessege = ResMessege.getDanger("Veuiller remplir les champs vide s'il vous plaît");
                    return View(model);
                }
                else
                {

                    /*   BindDatalist(model);
                        string xml = "";
                        if (Request.InputStream != null)
                        {
                            StreamReader stream = new StreamReader(Request.InputStream);
                            string x = stream.ReadToEnd();
                            xml = HttpUtility.UrlDecode(x);
                        }*/
                    string urlwebcal = "https://formateur.forma2plus.com/auto.php/webservices/security/authenticate?";
                    string token = GetTokenFromUrl(model.numindividu); // Supposons que cette fonction retourne le jeton
                    string urlWithToken = $"{urlwebcal}token={token}";
                    ViewBag.urlwebcal = urlWithToken;
                    ViewBag.idInd = model.numindividu;
                    var xml= GetPHPXML(model);
                    ViewBag.Reponse = xml;
                    string datecrea = model.nom;
                    if (model.nom != null)
                    {
                       datecrea = model.nom.Substring(0, 2); // Récupérer les deux premières lettres
                        datecrea = datecrea.ToUpper();
                    }
                 //   if (model.nom != model.nom_change)
                 if(model.nom_change)
                    {
                        try
                        {
                            var senderemail = new MailAddress("suividemonstage@forma2plus.com", "Forma2plus");
                      //  var receiveremail = new MailAddress(model.mail, "Receiver");
                         var receiveremail = new MailAddress("hermone@forma2plus.com", "Receiver");
                       
                            var password = "hermone";
                            string subject = "Forma2plus - Mise à jour de votre profil - Nouveau lien d'accès";
                            //  string body = "Bonjour,<br/><br/> Votre profil a été mis à jour, et nous voulions nous assurer que vous avez accès aux informations les plus récentes.<br/><br/>Pour accéder à votre profil mis à jour, veuillez utiliser le lien suivant: https://extranet.forma2plus.com/Profil/Plogon/" + model.Numero+"/"+nom+ ".<br/><br/>Votre Identifiant: " + nom+ "<br/>Votre numéro de dossier : " + model.Numero+ ".<br/><br/>N\'hésitez pas à nous contacter si vous avez des questions ou des préoccupations concernant ces modifications.<br/><br/> Cordialement ";
                            string body = "Bonjour,\n\nVotre profil a été mis à jour, et nous voulions nous assurer que vous avez accès aux informations les plus récentes.\nPour accéder à votre profil mis à jour, veuillez utiliser le lien suivant : https://extranet.forma2plus.com/Profil/Plogon/" + model.Numero + "/" + nom + "\nVotre Identifiant: " + nom + "\nVotre numéro de dossier : " + model.Numero + ".\n\nN'hésitez pas à nous contacter si vous avez des questions.\n\nCordialement \n\nEquipe Forma2plus ";

                            var smtp = new SmtpClient
                            {
                                Host = "mail.forma2plus.com",
                                Port = 25,
                                EnableSsl = true,
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                UseDefaultCredentials = false,
                                Credentials = new NetworkCredential(senderemail.Address, password)

                            };
                            using (var mes = new MailMessage(senderemail, receiveremail)
                            {
                                Subject = subject,
                                Body = body,

                            })
                            {
                                smtp.Send(mes);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Gérer l'erreur ici
                            // Vous pouvez journaliser l'erreur, afficher un message à l'utilisateur, etc.

                            // Rediriger l'utilisateur vers une page d'erreur
                            return RedirectToAction("Plogon", "Home", new { id = model.Numero, datecrea = datecrea });
                        }
                    }
                    return RedirectToAction("Plogon", "Home", new { id = model.Numero , datecrea= datecrea });
                }
              
            }
            return View(model);           

        }
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        public ActionResult Form(StagiaireModel model, string receiverEmail, string message)
        {

            //if (ModelState.IsValid)
            //{
            
                List<StagiaireModel> data = StagiaireModel.Email(model.mail);

                if (data.Count > 0)
                {
                    ViewBag.ResMessege = ResMessege.getDanger("Votre email a déjà un compte");
                    return View();

                }
                else
                {
                    // try
                    // {
                    var obj = InsertStagiaire(model);
               // var obj2 = InsertIp(model.Numero);
                    var senderemail = new MailAddress("hermone@forma2plus.com", "Forma2plus");
                    var receiveremail = new MailAddress("hermone.professionnel@gmail.com", "Receiver");
                    var password = "hermone";
                    string subject = "Forma2plus";
                    string body = "Votre numero de dossier est " + model.Result + " https://extranet.forma2plus.com/profils/ ";
                    var smtp = new SmtpClient
                    {
                        Host = "mail.forma2plus.com",
                        Port = 25,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderemail.Address, password)

                    };
                    using (var mes = new MailMessage(senderemail, receiveremail)
                    {
                        Subject = subject,
                        Body = body,

                    })
                    {
                        smtp.Send(mes);
                    }
                    return RedirectToAction("success", "Home");

                    //    }
                    //    catch (Exception)
                    /*  {
                          ViewBag.ResMessege = ResMessege.getDanger (" Probleme d'envoie d'email");
                          return View();
                      }*/

                }
            }
          /*  else
            {

                ViewBag.ResMessege = ResMessege.getDanger("Toutes les champs sont obligatoire");
                return View();


            }
        }*/

        public ActionResult Form()
        {
            var objMax = cnn.Query<StagiaireModel>("SELECT*FROM stagiaire_profil where numero=(SELECT MAX(numero) FROM stagiaire_profil)");
            if (objMax != null)
            {
                StagiaireModel model = new StagiaireModel();
                model.Numero = objMax.FirstOrDefault().Numero;
                model.Result = model.Numero + 1;
                return View(model);
            }
            return View();

        }
        public int numeroIndividu()

        {
            var numindividus = 0;
          
          

                var objMax = cnn.Query<Individu>("SELECT*FROM Individus where RéfIndividu=(SELECT MAX(RéfIndividu) FROM Individus)");
                if (objMax != null)
                {
                    Individu model = new Individu();
                    model.RéfIndividu = objMax.FirstOrDefault().RéfIndividu;
                    numindividus = model.RéfIndividu + 1;

                    // return numindividus;
                 }
               
           
            return numindividus;

        }
        public bool InsertStagiaire(StagiaireModel model)
        {
             
        int rowsAffected = cnn.Execute("INSERT into stagiaire_profil (numero,nom,prenom,societe,telephone,mail) values (" + model.Result + ",'" + model.nom + "','" + model.prenom + "','" + model.societe + "','" + model.Telephone + "','" + model.mail + "');");
            int rowsAffectede = cnn.Execute("INSERT into PROFIL (numero) values (" + model.Result + ");");
            if (rowsAffected > 0)
            {
                return true;
            }
            return false;
        }
        public bool InsertIp(int id)

        {
             
            string ip = HttpContext.Request.UserHostAddress;
            string navigation = HttpContext.Request.UserAgent;
            string nomnav = getNav(navigation);
            //  string nomplat = getBetween(navigation ,start, " ");
            string plateform = "win";
           // nomnav = "Chrome" + nomnav;

          //  string plateform = getBetween(navigation, "Win", ";");
           // plateform = "Win" + plateform;
            int rowAffected = cnn.Execute("INSERT into IP_stagiaires (provenance_appli, refindividu,IP_base,Navigateur, date_creation, plateform) values ('EXTRANET',"+id+",'"+ip+"','"+nomnav+"',now(),'"+plateform +"' )");
            if (rowAffected>0)
            {
                return true;
            }
            return false;
           }
        
        public static string getNav(string strSource)
        {
            const int kNotFound = -1;
            const string BROWSER_UNKNOWN = "unknown";
            const string BROWSER_OPERA = "Opera";
            const string BROWSER_OPERA_MINI = "Opera Mini";
            const string BROWSER_WEBTV = "WebTV";
            const string BROWSER_IE = "Internet Explorer";
            const string BROWSER_POCKET_IE = "Pocket Internet Explorer";
            const string BROWSER_KONQUEROR = "Konqueror";
            const string BROWSER_ICAB = "iCab";
            const string BROWSER_OMNIWEB = "OmniWeb";
            const string BROWSER_FIREBIRD = "Firebird";
            const string BROWSER_FIREFOX = "Firefox";
            const string BROWSER_ICEWEASEL = "Iceweasel";
            const string BROWSER_SHIRETOKO = "Shiretoko";
            const string BROWSER_MOZILLA = "Mozilla";
            const string BROWSER_AMAYA = "Amaya";
            const string BROWSER_LYNX = "Lynx";
            const string BROWSER_SAFARI = "Safari";
            const string BROWSER_IPHONE = "iPhone";
            const string BROWSER_IPOD = "iPod";
            const string BROWSER_IPAD = "iPad";
            const string BROWSER_CHROME = "Chrome";
            const string BROWSER_ANDROID = "Android";
            const string BROWSER_GOOGLEBOT = "GoogleBot";
            const string BROWSER_SLURP = "Yahoo! Slurp";
            const string BROWSER_W3CVALIDATOR = "W3C Validator";
            const string BROWSER_BLACKBERRY = "BlackBerry";
            const string BROWSER_ICECAT = "IceCat";
            const string BROWSER_NOKIA_S60 = "Nokia S60 OSS Browser";
            const string BROWSER_NOKIA = "Nokia Browser";
            const string BROWSER_MSN = "MSN Browser";
            const string BROWSER_MSNBOT = "MSN Bot";
            const string BROWSER_BINGBOT = "Bing Bot";

            const string BROWSER_NETSCAPE_NAVIGATOR = "Netscape Navigator";
            const string BROWSER_GALEON = "Galeon";
            const string BROWSER_NETPOSITIVE = "NetPositive";
            const string BROWSER_PHOENIX = "Phoenix";

            

           /* var startIdx = strSource.IndexOf(BROWSER_CHROME);
            var checkfirefox = strSource.IndexOf(BROWSER_FIREFOX);*/
            var checkopera = strSource.IndexOf(BROWSER_OPERA);
            var checkoperamini = strSource.IndexOf(BROWSER_OPERA_MINI);
            var checkwebtv = strSource.IndexOf(BROWSER_WEBTV);
            var checkbrowserie = strSource.IndexOf(BROWSER_IE); 
            var checkbrowserpoket_=strSource.IndexOf(BROWSER_POCKET_IE);
          var checkkonq= strSource.IndexOf(BROWSER_KONQUEROR);
            var checkicab = strSource.IndexOf(BROWSER_ICAB);
            var checkomni = strSource.IndexOf(BROWSER_OMNIWEB);
          var checkfire =strSource.IndexOf(BROWSER_FIREBIRD);
            var checkfiref = strSource.IndexOf(BROWSER_FIREFOX);
            var checkicew = strSource.IndexOf(BROWSER_ICEWEASEL);
            var checkshire = strSource.IndexOf(BROWSER_SHIRETOKO);
            var checkmoz = strSource.IndexOf(BROWSER_MOZILLA);
            var checkamay = strSource.IndexOf(BROWSER_AMAYA);
            var checklynx = strSource.IndexOf(BROWSER_LYNX);
            var checksaf = strSource.IndexOf(BROWSER_SAFARI);
            var checkiphone = strSource.IndexOf(BROWSER_IPHONE);
            var checkipod = strSource.IndexOf(BROWSER_IPOD);
            var chekcipad = strSource.IndexOf(BROWSER_IPAD);
            var checkchrome = strSource.IndexOf(BROWSER_CHROME);
            var checkandroid = strSource.IndexOf(BROWSER_ANDROID);
            var checkg = strSource.IndexOf(BROWSER_GOOGLEBOT);
            var checkslup = strSource.IndexOf(BROWSER_SLURP);

            var checkwval = strSource.IndexOf(BROWSER_W3CVALIDATOR);
            var checkblack = strSource.IndexOf(BROWSER_BLACKBERRY);
            var checkice = strSource.IndexOf(BROWSER_ICECAT);
            var checknokia = strSource.IndexOf(BROWSER_NOKIA_S60);
            var checknoki = strSource.IndexOf(BROWSER_NOKIA);
            var checkmsn = strSource.IndexOf(BROWSER_MSN);
            var checkmsnbot = strSource.IndexOf(BROWSER_MSNBOT);
           
            var checkbing= strSource.IndexOf(BROWSER_BINGBOT);

            var cheknav = strSource.IndexOf(BROWSER_NETSCAPE_NAVIGATOR);
            var checkgal = strSource.IndexOf(BROWSER_GALEON);
            var checknet = strSource.IndexOf(BROWSER_NETPOSITIVE);
           var checkphon= strSource.IndexOf(BROWSER_PHOENIX);
         /*   if (startIdx != kNotFound)
           '' {
                return BROWSER_CHROME;
            }
             if (checkfirefox != kNotFound)
                {
                return BROWSER_FIREFOX;
            }*/
             if (checkopera != kNotFound)
            {
                return BROWSER_OPERA;
            }
            if (checkoperamini != kNotFound)
            {
                return BROWSER_OPERA_MINI;
            }
            if (checkwebtv !=kNotFound)
            {
                return BROWSER_WEBTV;
            }
            if (checkbrowserie != kNotFound)
            {
                return BROWSER_IE;
            }
            if (checkbrowserpoket_ != kNotFound)
            {
                return BROWSER_POCKET_IE;
            }
            if (checkkonq != kNotFound)
            {
                return BROWSER_KONQUEROR;
            }
            if (checkicab != kNotFound)
            {
                return BROWSER_ICAB;
            }
            if (checkomni != kNotFound)
            {
                return BROWSER_OMNIWEB;
            }
            if (checkfire != kNotFound)
            {
                return BROWSER_FIREBIRD;
            }
            if (checkfiref != kNotFound)
            {
                return BROWSER_FIREFOX;
            }
            if (checkicew != kNotFound)
            {
                return BROWSER_ICEWEASEL;
            }
            if (checkshire != kNotFound)
            {
                return BROWSER_SHIRETOKO;
            }
            if (checkmoz != kNotFound)
            {
                return BROWSER_MOZILLA;
            }
            if (checkamay != kNotFound)
            {
                return BROWSER_AMAYA;
            }
            if (checklynx != kNotFound)
            {
                return BROWSER_LYNX;
            }
            if (checksaf != kNotFound)
            {
                return BROWSER_SAFARI;
            }
            if (checkiphone != kNotFound)
            {
                return BROWSER_IPHONE;
            }
            if (checkipod != kNotFound)
            {
                return BROWSER_IPOD;
            }
            if (chekcipad != kNotFound)
            {
                return BROWSER_IPAD;
            }
            if (checkchrome != kNotFound)
            {
                return BROWSER_CHROME;
            }
            if (checkandroid != kNotFound)
            {
                return BROWSER_ANDROID;
            }
            if (checkg != kNotFound)
            {
                return BROWSER_GOOGLEBOT;
            }
            if (checkslup != kNotFound)
            {
                return BROWSER_SLURP;
            }

            if (checkwval != kNotFound)
            {
                return BROWSER_W3CVALIDATOR;
            }

            if (checkblack != kNotFound)
            {
                return BROWSER_BLACKBERRY;
            }

            if (checkice != kNotFound)
            {
                return BROWSER_ICECAT;
            }

            if (checknokia != kNotFound)
            {
                return BROWSER_NOKIA_S60;
            }
            if (checknoki != kNotFound)
            {
                return BROWSER_NOKIA;
            }

            if (checkmsn != kNotFound)
            {
                return BROWSER_MSN;
            }

            if (checkmsnbot != kNotFound)
            {
                return BROWSER_MSNBOT;
            }

            if (checkbing != kNotFound)
            {
                return BROWSER_BINGBOT;
            }

            if (cheknav != kNotFound)
            {
                return BROWSER_NETSCAPE_NAVIGATOR;
            }

            if (checkgal != kNotFound)
            {
                return BROWSER_GALEON;
            }
            if (checknet != kNotFound)
            {
                return BROWSER_NETPOSITIVE;
            }

            if (checkphon != kNotFound)
            {
                return BROWSER_PHOENIX;
            }


            return BROWSER_UNKNOWN;







        }
        public string geturi(string urlp, string postData)
        {
            String Reponse = String.Empty;
            StreamReader Sr = null; // lire les données
            StreamWriter Sw = null; //Ecrire les données

            System.Uri Uri = new System.Uri(urlp);
            //ASCIIEncoding encoding = new ASCIIEncoding();
            //string postData = "ref_we=" + ref_we + "&amount=" + amount + "&ref_order=" + ref_order;
            //byte[] bytes;
            //bytes = System.Text.Encoding.ASCII.GetBytes(SampleXml);
            byte[] data = System.Text.Encoding.UTF8.GetBytes(postData);
            try
            {
          WebRequest httpWebRequest = WebRequest.Create(urlp);

                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 10000;
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.ContentLength = postData.Length;

                Sw = new StreamWriter(httpWebRequest.GetRequestStream());
                Sw.Write(postData);
                Sw.Close();
                Sw = null;


                WebResponse response = httpWebRequest.GetResponse();
                using (Sr = new StreamReader(response.GetResponseStream()))
                {
                    Reponse = Sr.ReadToEnd();
                    //Envoi de l'email
                    //sendEmailConfirmation(email, ref_we, ref_order, amount, Reponse);
                    Sr.Close();
                }


                return Reponse;

            }
            catch (Exception ex)
            {

              //  sendEmailConfirmationButErreur(email, ref_we, ref_order, amount, ex.Message);
                return ex.ToString();
            }
        }
      /*  public ActionResult smsapi()
        {

            BindDatalist(Url,postadata);
            string xml = "";
            if (Request.InputStream != null)
            {
                StreamReader stream = new StreamReader(Request.InputStream);
                string x = stream.ReadToEnd();
                xml = HttpUtility.UrlDecode(x);
            }

            return View();
        }*/

        private void  BindDatalist( StagiaireModel model)
        {
             

          var   url = "http://s371880604.onlinehome.fr/webcalendar/srcs/www/index.php?module=auto&action=import:index&PostedXMLStagiaire=";
            var StrXMLStagiaire = "<xml><stagiaires><stagiaire>";
            StrXMLStagiaire = StrXMLStagiaire + "<civilite> " + model.Civilite + "</civilite>";
            StrXMLStagiaire = StrXMLStagiaire + "<nom>" + model.nom + "</nom>";
            StrXMLStagiaire = StrXMLStagiaire + "<prenoms>" + model.prenom + "</prenoms>";
            StrXMLStagiaire = StrXMLStagiaire + "<mail>" + model.mail + "</mail>";
            StrXMLStagiaire = StrXMLStagiaire + "<fonction>" + model.fonction + "</fonction>";
            StrXMLStagiaire = StrXMLStagiaire + "<societe>" + model.societe + "</societe>";
            StrXMLStagiaire = StrXMLStagiaire + "<num_individu>" + model.numindividu + "</num_individu>";
            StrXMLStagiaire = StrXMLStagiaire + "<telephone>" + model.Telephone + "</telephone>";
            StrXMLStagiaire = StrXMLStagiaire + "<portable>" + model.Portable + "</portable>";
            StrXMLStagiaire = StrXMLStagiaire + "<rue_stage>" + model.RueDuStage + "</rue_stage>";
            StrXMLStagiaire = StrXMLStagiaire + "<ville_stage>" + model.VilleDuStage + "</ville_stage>";
            StrXMLStagiaire = StrXMLStagiaire + "<Cp_stage>" + model.CPDuStage + "</Cp_stage>";

            StrXMLStagiaire = StrXMLStagiaire + "</stagiaire> </stagiaires></xml>";

          /*  var client = new HttpClient(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
          // client.DefaultRequestHeaders = "application/xml";
            HttpResponseMessage responseMessage = client.SendAsync(StrXMLStagiaire);
           // var deserializedContent = responseMessage.Content.ReadAsDataContract<YourTypeHere>();
            //  var postData = "<xml>.....</xml>";*/

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(StrXMLStagiaire);
           // var xhtp =new HttpHeaders()
            var req = (HttpWebRequest)WebRequest.Create(url);

            req.ContentType = "text/xml";
            req.Method = "POST";
            req.ContentLength = bytes.Length;
            //req.A
            using (Stream os = req.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length);
            }

            string response = "";
            req.Timeout = 60000;
            // req.KeepAlive = true;
            using (System.Net.WebResponse resp = req.GetResponse())
            {
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    response = sr.ReadToEnd().Trim();
                }
            }
           // BindDatalist(Url, postadata);
          
            

        }
        /* private static HttpResponseMessage GetResponse(ServicePoint  host, HttpRequestMessage request)
         {
             /*  UriBuilder builder = new UriBuilder(host.BaseAddresses[0]);
               builder.Host = Environment.MachineName;

                request.RequestUri = new Uri(request.RequestUri.ToString(), UriKind.Relative);

                using (HttpClient client = new HttpClient(host))
                {
                    client.Channel = new WebRequestChannel();
                    return client.Send(request);
               }*/
        //  return request;
        //}

        public static Uri AttachParameters(Uri uri, NameValueCollection parameters)
        {
            var stringBuilder = new StringBuilder();
            string str = "&";
            for (int index = 0; index<parameters.Count; ++index)
            {
                stringBuilder.Append(str + parameters.AllKeys[index] + "=" + parameters[index]);
                str = "&";

            }
            return new Uri(uri + stringBuilder.ToString());
        }
        public static string GetPHP(string strUrl)

        {
            HttpClient client = new HttpClient();
         client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/Json"));
            var response =  client.GetAsync(strUrl).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;

            }
            else
            {
                return response.ReasonPhrase;

            }
        }
        public static string conversion(string textes)

        {
            var rep="";
            rep = WebUtility.HtmlDecode(textes);
            if (!(string.IsNullOrEmpty(rep)))
            {
                rep = rep.Replace("'", "''");
            }
            else
            {
                rep = ".";
            }
            return rep ;
        }
        public static string GetPHPXML(StagiaireModel model )

        {
          
           string content ="http://s371880604.onlinehome.fr/webcalendar/srcs/www/index.php?module=auto&action=import:index&PostedXMLStagiaire=";


                var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("civilite", model.Civilite));
            postData.Add(new KeyValuePair<string, string>("nom ", model.nom));
            postData.Add(new KeyValuePair<string, string>("prenoms ", model.prenom));
            postData.Add(new KeyValuePair<string, string>("mail ", model.mail));
            postData.Add(new KeyValuePair<string, string>("fonction ", model.fonction));
            postData.Add(new KeyValuePair<string, string>("societe", model.societe));
            postData.Add(new KeyValuePair<string, string>("num_individu ", (model.Numero).ToString()));
            postData.Add(new KeyValuePair<string, string>("portable ", model.Portable));
            postData.Add(new KeyValuePair<string, string>("Telephone ", model.Telephone));

            var StrXMLStagiaire = "<xml><stagiaires><stagiaire>";
            StrXMLStagiaire = StrXMLStagiaire + "<civilite> " + model.Civilite + "</civilite>";
            StrXMLStagiaire = StrXMLStagiaire + "<nom>" + model.nom + "</nom>";
            StrXMLStagiaire = StrXMLStagiaire + "<prenoms>" + model.prenom + "</prenoms>";
            StrXMLStagiaire = StrXMLStagiaire + "<mail>" + model.mail + "</mail>";
            StrXMLStagiaire = StrXMLStagiaire + "<fonction>" + model.fonction + "</fonction>";
            StrXMLStagiaire = StrXMLStagiaire + "<societe>" + model.societe + "</societe>";
            StrXMLStagiaire = StrXMLStagiaire + "<num_individu>" + model.numindividu + "</num_individu>";
            StrXMLStagiaire = StrXMLStagiaire + "<telephone>" + model.Telephone + "</telephone>";
            StrXMLStagiaire = StrXMLStagiaire + "<portable>" + model.Portable + "</portable>";
            StrXMLStagiaire = StrXMLStagiaire + "<rue_stage>" + model.RueDuStage + "</rue_stage>";
            StrXMLStagiaire = StrXMLStagiaire + "<ville_stage>" + model.VilleDuStage + "</ville_stage>";
            StrXMLStagiaire = StrXMLStagiaire + "<Cp_stage>" + model.CPDuStage + "</Cp_stage>";

            StrXMLStagiaire = StrXMLStagiaire + "</stagiaire> </stagiaires></xml>";

            HttpClient client = new HttpClient();
               client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/XML"));
            HttpContent contenu = new FormUrlEncodedContent(postData);
            var response = client.PostAsync( content,contenu).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;

            }
            else
            {
                return response.ReasonPhrase;

            }
        }
        /*private string ConvertTableauInt(int entree)
        {
            int l = entree;
            string sortie = new string l;

            for (int i = 0; i < l; i++)
                sortie[i] = entree[i].ToString();

            return sortie;

        }*/
        public   static string GetHtmlPage(string strURL)
         {

            String strResult;
            WebResponse objResponse;
            WebRequest objRequest = HttpWebRequest.Create(strURL);
            
            objResponse = objRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                strResult = sr.ReadToEnd();
                sr.Close();
            }
            return strResult;
        }
       /* public static string testenvoie(string strURL)
        {
            HttpClient client = new HttpClient();

            
                // Call asynchronous network methods in a try/catch block to handle exceptions.
                try
                {
                HttpResponseMessage response = await client.SendAsync ("http://www.contoso.com/");
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Above three lines can be replaced with new helper method below
                    // string responseBody = await client.GetStringAsync(uri);

                    Console.WriteLine(responseBody);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
            }*/
        
        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            const int kNotFound = -1;
            const string PLATFORM_UNKNOWN = "unknown";
            const string PLATFORM_WINDOWS = "Windows";
            const string PLATFORM_WINDOWS_CE = "Windows CE";
            const string PLATFORM_APPLE = "Apple";
            const string PLATFORM_LINUX = "Linux";
            const string PLATFORM_OS2 = "OS/2";
            const string PLATFORM_BEOS = "BeOS";
            const string PLATFORM_IPHONE = "iPhone";
            const string PLATFORM_IPOD = "iPod";
            const string PLATFORM_IPAD = "iPad";
            const string PLATFORM_BLACKBERRY = "BlackBerry";
            const string PLATFORM_NOKIA = "Nokia";
            const string PLATFORM_FREEBSD = "FreeBSD";
            const string PLATFORM_OPENBSD = "OpenBSD";
            const string PLATFORM_NETBSD = "NetBSD";
            const string PLATFORM_SUNOS = "SunOS";
            const string PLATFORM_OPENSOLARIS = "OpenSolaris";
            const string PLATFORM_ANDROID = "Android";
            var startIdx = strSource.IndexOf(strStart);
            if (startIdx != kNotFound)
            {
                startIdx += strStart.Length;
                var endIdx = strSource.IndexOf(strEnd, startIdx);
                if (endIdx > startIdx)
                {
                    return strSource.Substring(startIdx, endIdx - startIdx);
                }
            }
            return String.Empty;
        }

        private async Task<string> SendDataToHTTP(string url, string method, string data)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpMethod httpMethod = new HttpMethod(method.ToUpper());

                    HttpRequestMessage request = new HttpRequestMessage(httpMethod, new Uri(url));

                    if (httpMethod == HttpMethod.Post)
                    {
                        request.Content = new StringContent(data, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
                    }

                    HttpResponseMessage response = await httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        // Handle non-success status codes here
                        return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                return $"Exception: {ex.Message}";
            }
        }
    

    private string getProductLIstXML(string urls,string datas)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();

            string SampleXml = datas;

            try
            {
                byte[] data = encoding.GetBytes(SampleXml);

              //  string url = "http://www.domain2.com/products-xml";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urls);
                byte[] bytes;
                bytes = System.Text.Encoding.ASCII.GetBytes(SampleXml);
                request.ContentType = "text/xml; encoding='utf-8'";
                request.ContentLength = bytes.Length;
                request.Method = "POST";
                Stream requestStream =  request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    string responseStr = new StreamReader(responseStream).ReadToEnd();
                    return responseStr;
                }
                return null;

            }
            catch (WebException webex)
            {
                return webex.ToString();
            }
        }

        public string GenerateXMLStagiaireWebCalendar(int? idStagiaire, int numeroprofil)
        {
            string strXMLStagiaire = string.Empty;

           
                cnn.Open();

              // string sql = "SELECT Individus.RéfIndividu, Individus.Fonction , Individus.CodeTiers, Individus.Civilité, Individus.Nomfamille, Individus.Prénom, Individus.Département, Individus.Email, Individus.Adresse1 FROM Individus  WHERE(((Individus.RéfIndividu)=" + idStagiaire+"));";
          string  sql = "SELECT Individus.RéfIndividu, Individus.Nomfamille, Individus.Civilité, Individus.Prénom, Individus.Fonction, Individus.Tél, Individus.Adresse1, Individus.Ville, Individus.CodePostal, stagiaire_profil.numero , Individus.Email FROM Individus LEFT JOIN stagiaire_profil ON Individus.RéfIndividu = stagiaire_profil.numindividu WHERE(((Individus.RéfIndividu) = " + idStagiaire + "));";
                var stagiaire = cnn.QueryFirstOrDefault<IndividusModel>(sql);

                if (stagiaire != null)
                {
                    strXMLStagiaire += "<stagiaires><stagiaire>";
                    strXMLStagiaire += $"<civilite>{stagiaire.Civilité}</civilite>";
                    strXMLStagiaire += $"<nom>{stagiaire.Nomfamille}</nom>";
                    strXMLStagiaire += $"<prenoms>{stagiaire.Prénom}</prenoms>";
                    strXMLStagiaire += $"<mail>{stagiaire.Email}</mail>";
                    strXMLStagiaire += $"<fonction>{stagiaire.Fonction}</fonction>";
                    strXMLStagiaire += $"<societe>{stagiaire.CodeTiers}</societe>";
                    strXMLStagiaire += $"<num_individu>{stagiaire.RéfIndividu}</num_individu>";
                strXMLStagiaire += $"<telephone>{stagiaire.Tél}</telephone>";
                strXMLStagiaire += $"<idStagiaire>{stagiaire.numero}</idStagiaire>";


                // Add other properties as needed

                strXMLStagiaire += "</stagiaire></stagiaires>";
                }
            

            return strXMLStagiaire;
        }
        public string GenerateXMLStagiaireWebCalendarModel(StagiaireModel model)
        {
            string strXMLStagiaire = string.Empty;

            var telph1 = model.Portable;
            var telph2 = model.Telephone;

            if (telph2 == ".")
            {
                telph2 = telph1;
            }

            strXMLStagiaire += "<stagiaires><stagiaire>";
                strXMLStagiaire += $"<civilite>{model.Civilite}</civilite>";
                strXMLStagiaire += $"<nom>{model.nom}</nom>";
                strXMLStagiaire += $"<prenoms>{model.prenom}</prenoms>";
                strXMLStagiaire += $"<mail>{model.mail}</mail>";
                strXMLStagiaire += $"<fonction>{model.fonction}</fonction>";
                strXMLStagiaire += $"<societe>{model.societe}</societe>";
                strXMLStagiaire += $"<num_individu>{model.numindividu}</num_individu>";
                strXMLStagiaire += $"<telephone>{telph2}</telephone>";
                strXMLStagiaire += $"<idStagiaire>{model.Numero}</idStagiaire>";
            strXMLStagiaire += $"<ville>{model.VilleDuStage}</ville>";
            strXMLStagiaire += $"<portable>{model.Portable}</portable>";
            strXMLStagiaire += $"<rue_stage>{model.RueDuStage}</rue_stage>";
            strXMLStagiaire += $"<ville_stage>{model.VilleDuStage}</ville_stage>";



         
                // Add other properties as needed

            strXMLStagiaire += "</stagiaire></stagiaires>";
            


            return strXMLStagiaire;
        }
        public string postXMLData(string destinationUrl, string requestXml)
        {
            string postdata = "PostedXMLStagiaire=" + requestXml;
                
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
            byte[] bytes;
            //byte[] byteArray = Encoding.UTF8.GetBytes(postdata.ToString());
            bytes = System.Text.Encoding.ASCII.GetBytes(postdata.ToString());
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                string responseStr = new StreamReader(responseStream).ReadToEnd();
                return responseStr;
            }
            return null;
        }
       

    }


}
    