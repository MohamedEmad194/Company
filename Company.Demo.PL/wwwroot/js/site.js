// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let element = document.getElementById("id");

if (element) {
    element.addEventListener("input", () => {
        //send Request To Backend
        let xhr = new XMLHttpRequest();
        let url = `http://localhost:5000/Employee/Index?InputSearch=${element.value}`;
        xhr.open("post", url, true);

        xhr.onreadystatechange = function () {
            if (this.readyState == 4 && this.status == 200) {
                console.log(this.responseText);
                //Render Html Code
            }
        }
        //sending our request 
        xhr.send();
    });
}
