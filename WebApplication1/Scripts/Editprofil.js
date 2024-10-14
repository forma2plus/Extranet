
  
    
function suivant() {
    var mail = document.getElementById("mail").value;
    var telephone = document.getElementById("telephone");
    var portable = document.getElementById("portable").value;
   /*if (portable.value == "")
    {
        portable.focus();
    }*/
    /*if (mail == "" || portable == "") {
        document.getElementById("infos").style="display:block;color:red;";
      //  alert("Veuillez bien remplir tous les champs '*' svp! ");
     //   document.getElementById("button_suivant").href = "#v-pills-messages";


    } else {
        document.getElementById("infos").style("display:block");
    }*/

    
}
function check_somesZ() {
    var i = 0;
    var mail = document.getElementById("mail").value;
    var fonction = document.getElementById("fonction").value;
    var portable = document.getElementById("portable").value;
    if (mail == "") { alert("Veuillez remplir votre mail !"); i += 1; }
   // if (fonction == "") { alert("Veuillez remplir votre fonction"); i += 1; }
    if (portable == "") { alert("Veuillez remplir votre numero portable"); i += 1; }
    //envoiewebcal();
   // alert(i);
    if (i>0) {
       
        document.getElementById("infos").style = "display:block;color:red;";
    }
    else {
        document.getElementById("infos").style = "display:none;";
        //  alert("Veuillez bien remplir tous les champs '*' svp! ");
        document.getElementById("2").click();
        
    }
}
function check_dispoZ() {
    var i = 0;
    var horaisouhait = document.getElementById("horaisouhait").value;
    var Joursouhait = document.getElementById("Joursouhait").value;
   // var portable = document.getElementById("portable").value;
    if (horaisouhait == "") { i += 1; }
    if (Joursouhait == "") {  i += 1; }
   

    // alert(i);
    if (i > 0) {
        alert("Veuillez bien remplir votre disponibilité !");
        document.getElementById("infos2").style = "display:block;color:red;";
    }
    else {
        document.getElementById("infos2").style = "display:none;";
        //  alert("Veuillez bien remplir tous les champs '*' svp! ");
        document.getElementById("3").click() ;

    }
}
function check_lingustiquez() {
    var i = 0;
    var b = 0;
    var languemat = document.getElementById("languemat").value;
    var premlang = document.getElementById("premlang").value;
     var AttentesSpec = document.getElementById("AttentesSpec").value;
    if (languemat == "") { i += 1; }
    if (premlang == "") { i += 1; }
    if (AttentesSpec == "") { b += 1;}

    // alert(i);
    if (i > 0) {

        document.getElementById("infos3").style = "display:block;color:red;";
       
    }
    else {
        document.getElementById("infos3").style = "display:none;";
        //  alert("Veuillez bien remplir tous les champs '*' svp! ");
        document.getElementById("4").click() ;

    }
}
function check_some() {
    var mail = document.getElementById("mail");
    var fonction = document.getElementById("fonction");
    var portable = document.getElementById("portable");
    var premier = document.getElementById("languemat");
    if (mail.value == "") {
        alert("Veuillez bien remplir votre mail  svp!");
        document.getElementById('1').click();
        // mail.focus();
        //  return false;
    }
    else if (portable.value == "") {
        alert("Veuillez bien remplir votre numero portable svp!");
        document.getElementById('1').click();
        telephone.focus();

        //   check_somes();
        //return false;

    }
   /* else if (fonction.value == "") {
        alert("Veuillez indiquer votre fonction svp!");
        //   portable.focus();
        document.getElementById('1').click();

        //return false;
    }*/
    else if (premier.value == "") {
        alert("Veuillez bien remplir votre profil linguistique svp!");

        document.getElementById('3').click();
        premier.focus();
        // return false;
    }



    else {
        return true;
    }

}
function tester(champ) {
    var regex = /@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$/;
    if (!regex.test(champ)) {
        return true;
    }
}
function envoiewebcalsalle() {
    // var http = new XMLHttpRequest();
    var http = null;
    //XDomainRequest
    if ("XMLHttpRequest" in window) {
        http = new XMLHttpRequest();
    } else {
        // code for IE6, IE5
        http = new ActiveXObject("Microsoft.XMLHTTP");
    }
    var v_mail = document.getElementById("mail").value;
   
    var v_nom = document.getElementById("nom").value;
    var v_prenom = document.getElementById("prenom").value;
    var v_num = 0//document.getElementById("vide").value;

    var v_numero = document.getElementById("numeroprofil").value;
    
    var postData = "refIndividu=" + v_num + "&mdp=" + v_numero + "&nom=" + v_nom + "&prenom=" + v_prenom;
  
    var url = 'http://s371880604.onlinehome.fr//webcalendar_salle/srcs/www/auto.php?module=auto&action=import:updateStagiaire';

    var params = postData;

    alert("eto1");
    http.onreadystatechange = function () {//Call a function when the state changes.
        if (http.readyState == 4 && http.status == 200) {
            alert(http.responseText);
        }
        else {
            alert(http.responseText);
        }
    }
    http.open('POST', url, true);

    //Send the proper header information along with the request
    // http.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
    //  var allowOrigin = !string.IsNullOrWhiteSpace(origin) ? origin : "*";
    // http.Response.AddHeader("Access-Control-Allow-Origin", allowOrigin);
    //http.Response.AddHeader("Access-Control-Allow-Headers", "*");
    http.setRequestHeader("Access-Control-Allow-Credentials", "true");
    http.setRequestHeader('Content-type', 'application/json;charset=UTF-8');
    http.send(params);


}
function envoiewebcal() {
   // var http = new XMLHttpRequest();
    var http = null;
    //XDomainRequest
    if ("XMLHttpRequest" in window) {
        http = new XMLHttpRequest();
    } else {
        // code for IE6, IE5
        http = new ActiveXObject("Microsoft.XMLHTTP");
    }
    var v_mail = document.getElementById("mail").value;
    var v_fonct = document.getElementById("fonction").value;
    var v_port = document.getElementById("portable").value;
    var v_tel = document.getElementById("telephone").value;
    var v_nom = document.getElementById("nom").value;
    var v_prenom = document.getElementById("prenom").value;
    var v_societe = document.getElementById("societe").value;

    var v_numero = document.getElementById("numeroprofil").value;
    var v_RueDuStage = document.getElementById("RueDuStage").value;
    var v_civilite = document.getElementById("Civilite").value;

    var StrXMLStagiaire = "<xml><stagiaires><stagiaire>";
    StrXMLStagiaire = StrXMLStagiaire + "<civilite>" + v_civilite  + "<civilite>";
    StrXMLStagiaire = StrXMLStagiaire + "<nom>"  + v_nom +"</nom>";
    StrXMLStagiaire = StrXMLStagiaire+ "<prenoms>" +v_prenom  + "</prenoms>";
    StrXMLStagiaire = StrXMLStagiaire + "<mail>"+ v_mail + "</mail>";
    StrXMLStagiaire = StrXMLStagiaire + "<fonction>"+ v_fonct + "</fonction>";
    StrXMLStagiaire = StrXMLStagiaire + "<societe>"+ v_societe + "</societe>";
    StrXMLStagiaire = StrXMLStagiaire + "<num_individu>"+ v_numero + "</num_individu>";
    StrXMLStagiaire = StrXMLStagiaire + "<telephone>"+ v_tel + "</telephone>";
    StrXMLStagiaire = StrXMLStagiaire+ "<portable>"+ v_port + "</portable>";
    StrXMLStagiaire = StrXMLStagiaire + "<rue_stage>"+ v_RueDuStage + "</rue_stage>";
 //   StrXMLStagiaire = StrXMLStagiaire + "<ville_stage>"+ v_VilleDuStage + "</ville_stage>";
  //  StrXMLStagiaire = StrXMLStagiaire + "<Cp_stage>"+ v_CPDuStage + "</Cp_stage>";

    StrXMLStagiaire = StrXMLStagiaire + "</stagiaire> </stagiaires></xml>";
    var url = 'http://s371880604.onlinehome.fr/webcalendar/srcs/www/index.php?module=auto&action=import:index&PostedXMLStagiaire=';

    var params = StrXMLStagiaire;

    alert("eto");
    http.onreadystatechange = function () {//Call a function when the state changes.
        if (http.readyState == 4 && http.status == 200) {
            alert(http.responseText);
        }
        else {
            alert(http.responseText);
        }
    }
    http.open('POST', url, true);

    //Send the proper header information along with the request
   // http.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
  //  var allowOrigin = !string.IsNullOrWhiteSpace(origin) ? origin : "*";
   // http.Response.AddHeader("Access-Control-Allow-Origin", allowOrigin);
    //http.Response.AddHeader("Access-Control-Allow-Headers", "*");
    http.setRequestHeader("Access-Control-Allow-Credentials", "true");
    http.setRequestHeader('Content-type', 'application/json;charset=UTF-8');
    http.send(params);
    
  
}
function validation(event) {
    var unicode = e.keyCode ? e.keyCode : e.charCode
    if (unicode >= 48 && unicode <= 57) {

        return false;
    }
    else {
        return true;
    }
}
    function activate(e) {

        if (e.which > 31 && (e.which < 48 || e.which > 57) && e.which != 46) {
            return true;

        } else {
            e.preventDefault();
            return false;
        }
}
function surligne(erreur)//change de couleur selon la conformité de ce qui est rentré
{
    if (erreur) {
        champ.style.borderColor = "#D10C13";
        champ.style.border = "2";
       
        }
    else {
        champ.style.borderColor = "#04DC13";
    }
}
    function verifPrenom(champ)//va verifier le champs
{
  var regex = /^[a-zA-Zéèïîê]+[ \-']?[[a-zA-Zéèïîê]+[ \-']?]*[a-zA-Zéèïîê]$/;
  if(!regex.test(champ.value))
  {
                surligne(champ, true);
    return false;
   }
  else
  {
      surligne(champ, false);
            return true;
          }
            }
