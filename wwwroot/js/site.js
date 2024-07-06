
//Carousel.TRANSITION_DURATION = 1000 /* Your number here*/
//Carousel.DEFAULTS = {
//    interval: 7000, /* you could change this value too, but to add data-interval="xxxx" to your html it's fine too*/
//        pause: 'hover',
//    wrap: true,
//    keyboard: true
//}

// *********************************************************************************
//
// **   STICKY NAV BAR                                          STICKY NAV BAR
//
// ********************************************************************************

$(window).scroll(function () {
    if ($(window).scrollTop() >= 48) {
        $('#stickyNav').addClass('fixed-header');
    }
    else {
        $('#stickyNav').removeClass('fixed-header');
    }
});

// *********************************************************************************
//
// **   RESPONSIVE TEXT                                             RESPONSIVE TEXT
//
// ********************************************************************************
// class element you need the width from
const cardHeader = document.querySelectorAll('.myCard');

const myObserver = new ResizeObserver(function (entry) {
    for (let i = 0; i < entry.length; i++) {
        const width = entry[i].contentRect.width;
        const height = entry[i].contentRect.height;

        // If your cards use a filter, when it runs, the observer may lose the width of the card, making it 0;
        // The below manually forces a resize so the observer fires again;
        // Then the card will change to its respoonsive size, the transition will be imperceptible;
        if (width == 0) {
            if (cardHeader.style != undefined) {
                cardHeader.style.width = 400;
            } else {
                continue;
            }
        }

        // responsive text size
        let headerTextSize = Math.round(width * .1) + "px"; // basically a percentage of width
        let headerTextMax = 30 + "px"; // set max text size
        let headerTextMin = 24 + "px"; // set min text size

        let bodyTextSize = Math.round(width * .08) + "px";
        let bodyTextMax = 18 + "px"; // set max text size
        let bodyTextMin = 12 + "px"; // set min text size

        let buttonTextSize = Math.round(width * .08) + "px";
        let buttonTextMax = 22 + "px"; // set max text size
        let buttonTextMin = 17 + "px"; // set min text size

        let buttonPaddingSize = Math.round(width * .05) + "px";
        let buttonPaddingMax = 8 + "px"; // set max padding
        let buttonPaddingMin = 3 + "px"; // set min padding size

        // for displaying the numbers
        // document.getElementById("width").innerHTML = "Header Text: " + headerTextSize + "&nbsp; --- Body Text: " + bodyTextSize + "&nbsp; --- Observe Area Width: " + Math.round(width) + "px";

        let headerText = document.querySelectorAll(".card-title");

        if (width > 800) {
            headerText.forEach(el => { el.style.fontSize = headerTextMax; });
        } else if (width < 50) {
            headerText.forEach(el => { el.style.fontSize = headerTextMin; });
        } else {
            headerText.forEach(el => { el.style.fontSize = headerTextSize; });
        }

        let bodyText = document.querySelectorAll(".card-text");

        if (width > 800) {
            bodyText.forEach(el => { el.style.fontSize = bodyTextMax; });
        } else if (width < 200) {
            // bodyText.forEach(el => {el.style.fontSize = bodyTextMin;});
            bodyText.forEach(el => { el.style.display = 'none'; });
        } else {
            bodyText.forEach(el => {
                el.style.fontSize = bodyTextSize;
                el.style.display = '-webkit-box';
            });
        }

        let buttonText = document.querySelectorAll(".myCardButton");

        if (width > 800) {
            buttonText.forEach(el => {
                el.style.fontSize = buttonTextMax;
                el.style.padding = buttonPaddingMax;
            });
        } else if (width < 50) {
            buttonText.forEach(el => {
                el.style.fontSize = buttonTextMin;
                el.style.padding = buttonPaddingMin;
            });
        } else {
            buttonText.forEach(el => {
                el.style.fontSize = buttonTextSize;
                el.style.padding = buttonPaddingSize;
            });
        }
    }
});

cardHeader.forEach(header => {
    myObserver.observe(header);
});


// **************************************************
//
//              BACK TO TOP BUTTON
//
// **************************************************

$(document).ready(function () {
    $("h1").delay("1000").fadeIn();
    // hide #back-top first
    $("#btt").hide();

    // fade in #back-top
    $(function () {
        $(window).scroll(function () {
            if ($(this).scrollTop() > 200) {
                $('#btt').fadeIn(800);
            } else {
                $('#btt').fadeOut(400);

            }
        });

        // scroll body to 0px on click
        $('a#btt').click(function () {
            $('body,html').animate({
                scrollTop: 0
            }, 800);
            return false;
        });
    });
});

// **************************************************
//
//              DETAIL VIEW UPDATE INFO FORMS
//
// **************************************************

document.getElementById('updateInfo').addEventListener('click', (e) => {
    e.preventDefault();
    var myInfo = document.getElementById('myInfo');
    var updateMovie = document.getElementById('updateMovie');

    updateMovie.style.display = 'block';
    myInfo.style.display = 'none';
});

document.getElementById('saveInfo').addEventListener('click', () => {
    var myInfo = document.getElementById('myInfo');
    var updateMovie = document.getElementById('updateMovie');

    myInfo.style.display = 'block';
    updateMovie.style.display = 'none';
});

document.getElementById('cancelInfo').addEventListener('click', (e) => {
    e.preventDefault();
    var myInfo = document.getElementById('myInfo');
    var updateMovie = document.getElementById('updateMovie');

    myInfo.style.display = 'block';
    updateMovie.style.display = 'none';
});

