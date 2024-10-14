using Dapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Odbc;
using System.Linq;
using System;

namespace WebApplication1.Models
{
    public class StagiaireModel
    {
        public int Numero { get; set; }
     
        [Display(Name = "nom")]
        [Required(ErrorMessage = "Veuillez entrer votre nom")]
        public string nom { get; set; }

  
        [Display(Name = "prenom")]
        [Required(ErrorMessage = "Veuillez  entrer votre prenom")]
        public string prenom { get; set; }
       
        [Display(Name = "Societe")]
        [Required(ErrorMessage = "Veuillez  entrer votre nom de Société ")]
        public string societe { get; set; }
        [Display(Name = "Telephone")]
        public string Telephone { get; set; }

        [Display(Name = "Mail")]
        [Required(ErrorMessage = "Veuillez  entrer votre E-mail")]
        public string mail { get; set; }


        public string fonction { get; set; }
        public string Departement { get; set; }

        [Required(ErrorMessage = "Veuillez indiquer votre numero portable")]
         public string Portable { get; set; }
        public string Civilite { get; set; }
        public string joursouhait { get; set; }
        public string horaisouhait { get; set; }
        public string tempsperso { get; set; }
        public string tempsprof { get; set; }
        public string RueDuStage { get; set; }
        public string VilleDuStage { get; set; }
        public string CPDuStage { get; set; }
        public bool accesnetperso { get; set; }
        public bool accesnetpro { get; set; }
        public bool E_learning { get; set; }
       
        public string languemat { get; set; }
        public bool niveausco { get; set; }
        public string nbansbac { get; set; }
        public string premlang { get; set; }
        public string paysanglo { get; set; }
        public bool LangGen { get; set; }
        public bool LangPro { get; set; }
        public string NationInterloc { get; set; }
        public string FonctInterloc { get; set; }
      
        public string BesoinsSpecif { get; set; }
        public string AttentesSpec { get; set; }
        public string formatAnt { get; set; }
        public bool AttentesGramm { get; set; }
        public bool AttentesCompreh { get; set; }
        public bool AttentesVocab { get; set; }
        public bool ConfrAccueilVisite { get; set; }
        public bool ConfrTel { get; set; }
        public bool ConfrReunion { get; set; }
        public bool ConfrOral { get; set; }
        public bool ConfrLecture { get; set; }
        public bool ConfrRedact { get; set; }
        public bool ConfrDeplace { get; set; }
        public bool ConfrPresent { get; set; }
        public bool ConfrNegoc { get; set; }
        public string ObjStage { get; set; }
        public bool sport { get; set; }
        public bool jardin { get; set; }
        public bool musique { get; set; }
        public bool theatre { get; set; }
        public bool arts { get; set; }
        public bool sciences { get; set; }
        public bool litterature { get; set; }
        public bool bricolage { get; set; }
        public bool cuisine { get; set; }
        public string autres_interets { get; set; }
        public string nbabscprevue { get; set; }
        public int Result { get; set; }
        public bool Cinema_Arts1 { get; set; }
        public bool Cinema_Arts2 { get; set; }
        public bool Cinema_Arts3 { get; set; }
        public bool Cinema_Arts4 { get; set; }
        public bool Cinema_Arts5 { get; set; }
        public bool Cinema_Arts6 { get; set; }
        public bool Cinema_Arts7 { get; set; }
        public bool Cinema_Arts8 { get; set; }
        public bool Cinema_Arts9 { get; set; }
        public bool Cinema_Arts10 { get; set; }
        public bool Cinema_Arts11 { get; set; }

        public bool Cinema_Arts12 { get; set; }
        public bool Cinema_Arts13 { get; set; }
        public bool Travelling_Lifestyle1 { get; set; }
        public bool Travelling_Lifestyle2 { get; set; }
        public bool Travelling_Lifestyle3 { get; set; }
        public bool Travelling_Lifestyle4 { get; set; }
        public bool Travelling_Lifestyle5 { get; set; }
        public bool Travelling_Lifestyle6 { get; set; }
        public bool Travelling_Lifestyle7 { get; set; }
        public bool Travelling_Lifestyle8 { get; set; }
        public bool Travelling_Lifestyle9 { get; set; }
        public bool Travelling_Lifestyle10 { get; set; }
        public bool Health_Fitness_Sport1 { get; set; }
        public bool Health_Fitness_Sport2 { get; set; }
        public bool Health_Fitness_Sport3 { get; set; }
        public bool Health_Fitness_Sport4 { get; set; }
        public bool Health_Fitness_Sport5 { get; set; }
        public bool Crafts_Hobbies1 { get; set; }
        public bool Crafts_Hobbies2 { get; set; }
        public bool Crafts_Hobbies3 { get; set; }
        public bool Crafts_Hobbies4 { get; set; }
        public bool Crafts_Hobbies5 { get; set; }
        public bool Crafts_Hobbies6 { get; set; }
        public bool Nature_Outdoors1 { get; set; }
        public bool Nature_Outdoors2 { get; set; }
        public bool Nature_Outdoors3 { get; set; }
        public bool Politics_Business1 { get; set; }
        public bool Politics_Business2 { get; set; }
        public bool Politics_Business3 { get; set; }
        public bool Politics_Business4 { get; set; }
        public bool Politics_Business5 { get; set; }
        public bool Politics_Business6 { get; set; }
        public bool Politics_Business7 { get; set; }

        public bool Science_Technology1 { get; set; }
        public string type_mail { get; set; }
        public bool Science_Technology2 { get; set; }
        public bool Parenting_Family1 { get; set; }
        public bool Parenting_Family2 { get; set; }
        public bool Parenting_Family3 { get; set; }
        public int refindividu { get; set; }
        public int? numindividu { get; set; }
        public string IP_base { get; set; }
        public string Navigateur { get; set; }
        public string provenance_appli { get; set; }
        public string plateform { get; set; }
        public int numero_idbase { get; set; }
        public DateTime Date_creation { get; set; }
        public int Note_globale { get; set; }
        //  public string nom_change { get; set; }
         public bool nom_change { get; set; }

        public List<SocieteModel> ListeDesSocietes { get; set; }

        public void ChargerListeDesSocietes()
        {
            ListeDesSocietes = GetAllSocietes();
        }

        public static string GetConnectionString(string connectionName = "format")
        {
            return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
        }
        public StagiaireModel()
        {

        }

        public static List<StagiaireModel> Index(int numero, string mail)
        {

            using (OdbcConnection cnn = new OdbcConnection(GetConnectionString()))
            {

                //return cnn.Query<StagiaireModel>(@"select S.numero,numindividu,civilite,fonction,telephone,prenom,nom,departement,portable,mail,societe,joursouhait,tempsprof,RueDuStage,VilleDuStage,CPDuStage,accesnetpro,horaisouhait,e_learning,accesnetperso,P.numero,niveausco,languemat,nbansbac,premlang,paysanglo,LangGen,LangPro,NationInterloc,FonctInterloc,BesoinsSpecif,AttentesSpec  from STAGIAIRE S right JOIN PROFIL P ON S.numero =P.numero where (S.numero=" + numero + ");").ToList();

                // return cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, PROFIL.*, personal_interest.*, stagiaire_profil.numero FROM (stagiaire_profil LEFT JOIN personal_interest ON stagiaire_profil.numero = personal_interest.numero_profil) LEFT JOIN PROFIL ON stagiaire_profil.numero = PROFIL.numero WHERE (((stagiaire_profil.numero) = " + numero + "));").ToList();
                 return cnn.Query<StagiaireModel>(@" select * from stagiaire_profil where mail='" + mail + "' AND numero= " + numero + ";").ToList();

            }
        }
        public static List<StagiaireModel> Login(int numero)
        {

            using (OdbcConnection cnn = new OdbcConnection(GetConnectionString()))
            {

                //return cnn.Query<StagiaireModel>(@"select S.numero,numindividu,civilite,fonction,telephone,prenom,nom,departement,portable,mail,societe,joursouhait,tempsprof,RueDuStage,VilleDuStage,CPDuStage,accesnetpro,horaisouhait,e_learning,accesnetperso,P.numero,niveausco,languemat,nbansbac,premlang,paysanglo,LangGen,LangPro,NationInterloc,FonctInterloc,BesoinsSpecif,AttentesSpec  from STAGIAIRE S right JOIN PROFIL P ON S.numero =P.numero where (S.numero=" + numero + ");").ToList();

                 return cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, PROFIL.*, personal_interest.*, stagiaire_profil.numero FROM (stagiaire_profil LEFT JOIN personal_interest ON stagiaire_profil.numero = personal_interest.numero_profil) LEFT JOIN PROFIL ON stagiaire_profil.numero = PROFIL.numero WHERE (((stagiaire_profil.numero) = " + numero + "));").ToList();
               // return cnn.Query<StagiaireModel>(@" select * from stagiaire_profil where mail='" + mail + "' AND numero= " + numero + ";").ToList();

            }
        }
        public static List<StagiaireModel> Email(string mail)
        {

            using (OdbcConnection cnn = new OdbcConnection(GetConnectionString()))
            {

                return cnn.Query<StagiaireModel>(@"select * from stagiaire_profil where mail='" + mail + "';").ToList();
            }
        }
        public static List<StagiaireModel> NumeroDossier(string mail)
        {

            using (OdbcConnection cnn = new OdbcConnection(GetConnectionString()))
            {

                return cnn.Query<StagiaireModel>(@"select numero, nom, prenom from stagiaire_profil where mail='" + mail + "' ORDER BY numero DESC;").ToList();
            }
        }
        public static List<StagiaireModel> Personnal(int numero)
        {

            using (OdbcConnection cnn = new OdbcConnection(GetConnectionString()))
            {


                return cnn.Query<StagiaireModel>(@"SELECT personal_interest.*, personal_interest.numero_profil FROM personal_interest WHERE (((personal_interest.numero_profil)=" + numero + "));").ToList();
            }

        }
        public static List<StagiaireModel> Noteglobal(int numero)
        {
            using (OdbcConnection cnn = new OdbcConnection(GetConnectionString()))
            {
                return cnn.Query<StagiaireModel>(@"SELECT test_en_ligne.numero_idbase, test_en_ligne.* FROM test_en_ligne WHERE(((test_en_ligne.numero_idbase) = " + numero + "));").ToList();
            }
        }
        public static List<StagiaireModel> Profil(int numero)
        {
            using (OdbcConnection cnn = new OdbcConnection(GetConnectionString()))
            {
                return cnn.Query<StagiaireModel>(@"SELECT PROFIL.numero,  PROFIL.*FROM PROFIL WHERE (((PROFIL.numero)= " + numero + "));").ToList();
            }
        }
        public static List<StagiaireModel> Permissions(int numero)
        {
            using (OdbcConnection cnn = new OdbcConnection(GetConnectionString()))
            {
                return cnn.Query<StagiaireModel>(@"SELECT PROFIL.numero,  PROFIL.*FROM PROFIL WHERE (((PROFIL.numero)= " + numero + " AND ((PROFIL.languemat)<>'')));").ToList();
            }
        }
        public static List<StagiaireModel> PermssionTel(int numero)
        {
            using (OdbcConnection cnn = new OdbcConnection(GetConnectionString()))
            {
                return cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, stagiaire_profil.numero, stagiaire_profil.Telephone, stagiaire_profil.mail, stagiaire_profil.nom, stagiaire_profil.prenom FROM stagiaire_profil WHERE (((stagiaire_profil.numero)= " + numero + ") AND ((stagiaire_profil.Portable)<>'') AND ((stagiaire_profil.mail)<>'') AND ((stagiaire_profil.nom)<>'') AND ((stagiaire_profil.prenom)<>''));").ToList();
            }
        }
        public static List<StagiaireModel> PermssionFonction(int numero)
        {
            using (OdbcConnection cnn = new OdbcConnection(GetConnectionString()))
            {
                return cnn.Query<StagiaireModel>(@"SELECT stagiaire_profil.*, stagiaire_profil.numero, stagiaire_profil.Telephone, stagiaire_profil.mail, stagiaire_profil.nom, stagiaire_profil.prenom FROM stagiaire_profil WHERE (((stagiaire_profil.numero)= " + numero + ") AND ((stagiaire_profil.fonction)<>'') AND ((stagiaire_profil.mail)<>'') AND ((stagiaire_profil.nom)<>'') AND ((stagiaire_profil.prenom)<>''));").ToList();
            }
        }

        public static List<StagiaireModel> Linguistique(int numero)
        {

            using (OdbcConnection cnn = new OdbcConnection(GetConnectionString()))
            {


                return cnn.Query<StagiaireModel>(@"SELECT PROFIL.*, PROFIL.numero FROM PROFIL WHERE (((PROFIL.numero)=" + numero + "));").ToList();
            }

        }
        public static List<StagiaireModel>Individus(int numero)
        {

            using (OdbcConnection cnn = new OdbcConnection(GetConnectionString()))
            {


                return cnn.Query<StagiaireModel>(@"select * from stagiaire_profil where numero="+numero + "));").ToList();
            }

        }
        public static List<SocieteModel> GetAllSocietes()
        {
            using (OdbcConnection cnn = new OdbcConnection(GetConnectionString()))
            {


                return cnn.Query<SocieteModel>(@"SELECT CodeTiers, [Dénomination sociale] as Dénomination FROM Société ORDER BY Société.CodeTiers").ToList();
            }
           
        }




    }
}

public class Individu
{
    public int RéfIndividu { get; set; }
    public string CodeTiers { get; set; }
    public string Civilité { get; set; }
    public string NomFamille { get; set; }
    public string Prénom { get; set; }

    public string Email { get; set; }
    public string Tél { get; set; }
    public string Adresse1 { get; set; }
    public string Ville { get; set; }
    public int Note_globale { get; set; }
    
    public static string GetConnectionString(string connectionName = "format")
    {
        return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
    }
    public Individu()
    {

    }
    public static List<Individu> Individus(int numero)
    {
        using (OdbcConnection cnn = new OdbcConnection(GetConnectionString()))
        {


            return cnn.Query<Individu>(@"SELECT stagiaire_profil.* FROM stagiaire_profil WHERE (((stagiaire_profil.numero)=" + numero + "));").ToList();
        }
        


}
    public static List<Individu> Noteglobal(int numero)
    {
        using (OdbcConnection cnn = new OdbcConnection(GetConnectionString()))
        {


            return cnn.Query<Individu>(@"SELECT stagiaire_profil.numero, stagiaire_profil.nom, stagiaire_profil.prenom, test_en_ligne.date_passation, test_en_ligne.heure_passation, test_en_ligne.temps_passation, test_en_ligne.Note_globale
FROM test_en_ligne INNER JOIN stagiaire_profil ON test_en_ligne.numero_idbase = stagiaire_profil.numero
WHERE (((stagiaire_profil.numero)=" + numero + "));").ToList();
        }

    }



}