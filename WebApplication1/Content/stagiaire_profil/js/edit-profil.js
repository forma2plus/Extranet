let btn_next = document.querySelectorAll("[data-btn = 'next']");

btn_next[0].addEventListener('click', (e) => {
    e.preventDefault;
    var i = 0;
    var mail = document.getElementById("mail").value;
    var fonction = document.getElementById("fonction").value;
    var portable = document.getElementById("portable").value;
    if (mail == "") { alert("Veuillez remplir votre mail !"); i += 1; }
    // if (fonction == "") { alert("Veuillez remplir votre fonction"); i += 1; }
    if (portable == "") { alert("Veuillez remplir votre numero portable"); i += 1; }
    //envoiewebcal();
    // alert(i);
    if (i > 0) {

        document.getElementById("infos").style = "display:block;color:red;";
    }
    else {
        document.getElementById("infos").style = "display:none;";
        //  alert("Veuillez bien remplir tous les champs '*' svp! ");
     //  document.getElementById("2").click();
        let btn_tab = e.target.getAttribute("aria-controls")

        document.getElementById(btn_tab).click()

    }

   

});

btn_next[1].addEventListener('click', (e) => {
    e.preventDefault;

    var i = 0;
    var horaisouhait = document.getElementById("horaisouhait").value;
    var Joursouhait = document.getElementById("Joursouhait").value;
    // var portable = document.getElementById("portable").value;
    if (horaisouhait == "") { i += 1; }
    if (Joursouhait == "") { i += 1; }


    // alert(i);
    if (i > 0) {
      //  alert("Veuillez bien remplir votre disponibilité !");
        document.getElementById("infos2").style = "display:block;color:red;";
    }
    else {
        document.getElementById("infos2").style = "display:none;";
        //  alert("Veuillez bien remplir tous les champs '*' svp! ");
        //document.getElementById("3").click();
        let btn_tab = e.target.getAttribute("aria-controls")

        document.getElementById(btn_tab).click()

    }
    

});

btn_next[2].addEventListener('click', (e) => {
    e.preventDefault;

    var i = 0;
    var b = 0;
    var languemat = document.getElementById("languemat").value;
    var premlang = document.getElementById("premlang").value;
    var AttentesSpec = document.getElementById("AttentesSpec").value;
    if (languemat == "") { i += 1; }
    if (premlang == "") { i += 1; }
    if (AttentesSpec == "") { b += 1; }

    // alert(i);
    if (i > 0) {

        document.getElementById("infos3").style = "display:block;color:red;";

    }
    else {
        document.getElementById("infos3").style = "display:none;";
        //  alert("Veuillez bien remplir tous les champs '*' svp! ");
    //    document.getElementById("4").click();
        let btn_tab = e.target.getAttribute("aria-controls")

        document.getElementById(btn_tab).click()

    }
    

});
