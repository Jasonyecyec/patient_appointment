const userProfile = document.querySelector(".navbar-user-name");
const navbarMenuContainer = document.querySelector(".navbar-user-menu-container");

let isClicked = false;

//userProfile.addEventListener('click', function () {
//    navbarMenuContainer.classList.remove("display-navbar-menu")
//    isClicked = true;
//})

document.body.addEventListener("click", function (e) {
    //if userprofile is click show container, then set isChanged to true


    if (isClicked) {
          if (e.target.closest(".navbar-user-menu-container")) {
             console.log("yes")
            } else {
                 navbarMenuContainer.classList.add("display-navbar-menu")
                    isClicked = false;
                     console.log("no")
         }
    }

    if (e.target == userProfile) {
            navbarMenuContainer.classList.remove("display-navbar-menu")
            isClicked = true;
            console.log("view")
   

    }


  
})