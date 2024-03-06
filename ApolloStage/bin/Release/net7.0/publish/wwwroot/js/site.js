var registerPages = ["email", "name_pass_username", "birthday_country_gender"];
var index = 0;

function nextPage() {
    document.getElementById("login_google_OR").classList.add("d-none");
    if (registerPages[index+1] == "birthday_country_gender") {
        document.getElementById("registerSubmit").classList.add("d-none");
        document.getElementById("registerSubmit").classList.remove("d-block");
        document.getElementById("registerConfirmation").classList.remove("d-none");
        document.getElementById("registerConfirmation").classList.add("d-block");

    }
    document.getElementById(registerPages[index]).classList.add("d-none");
    document.getElementById(registerPages[index]).classList.remove("d-block");
    document.getElementById(registerPages[index + 1]).classList.remove("d-none");
    document.getElementById(registerPages[index + 1]).classList.add("d-block");
    index++;
   
}

function checkNumber(code) {
    var obj = document.getElementById(code);
    if (/^[0-9]*$/.test(parseInt(obj.value))) {
    } else {
        obj.value = " ";
    }
}


var digitPeriodRegExp = new RegExp('\\d|\\.');

var fields = document.getElementsByClassName("code");
for (const v of fields) {
    
    v.addEventListener('keydown', function (event) {
        if (event.ctrlKey // (A)
            || event.altKey // (A)
            || typeof event.key !== 'string' // (B)
            || event.key.length !== 1) { // (C)
            return;
        }

        if (!digitPeriodRegExp.test(event.key)) {
            console.log(1);
            event.preventDefault();
        }
    }, false);
    
}

var container = document.getElementsByClassName("codes")[0];
container.onkeyup = function (e) {
    var target = e.srcElement;
    var maxLength = parseInt(target.attributes["maxlength"].value, 10);
    var myLength = target.value.length;
    if (myLength >= maxLength) {
        var next = target;
        while (next = next.nextElementSibling) {
            if (next == null)
                break;
            if (next.tagName.toLowerCase() == "input") {
                next.focus();
                break;
            }
        }
    }
}

