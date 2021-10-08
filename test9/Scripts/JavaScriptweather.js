$(document).ready(function () {



   // $(document).on("contextmenu", function () {
     //   return false;
  //  });



    $(document).on("mousedown", function (event) {

        if (event.which == 3) {

            $(".hidden").show();
            $(".hiddenimg").hide();
            if ($(event.target).is("img")) {

                $(".hidden").hide();
                $(".hiddenimg").show();

            }

            //if (!$(event.target).is('img')) {
            //    $(".hidden").show();
            //$(".hiddenimg").hide();
            //if ($(event.target).is("img")) {

            //    $(".hidden").hide();
            //    $(".hiddenimg").show();

            //}






            $("#context").css({
                top: event.pageY,
                left: event.pageX
            })
            $("#context").fadeIn();
            return false;
        }
        $("#context").fadeOut();

    });





    var $image = $('#prognoz');
    var $video = $('#myvideo');

    $(function () {
        $image.on('click', function () {
            if (document.getElementById("prognoz")) {
                $image.hide();
                $video.show();

            }
        });

        $(function () {
            $video.on('click', function () {

                $image.show();
                $video.hide();


            });
        });
    });


    document.querySelector("#SendButtonn").onclick = function () {
        //alert("Вы нажали на кнопку");
        $('.any').css
            (
            {
                'display': 'block',
                'visibility': 'visible',
                'box-suppress': 'show'
            }
            );

    }







});

// Выпадающее меню
$("[data-trigger='dropdown']").on("mouseenter", function () {
    var submenu = $(this).parent().find(".submenu");
    //submenu.addClass("active");
    submenu.fadeIn(300);
    document.getElementById("dropdowncontent").style.display = "none"
    $("#may").on("mouseenter", function () {
        document.getElementById("dropdowncontent").style.display = "block"

    });

    $(".menu").on("mouseleave", function () {
        //submenu.removeClass("active");
        submenu.fadeOut(300);
    })
});






var btn = document.getElementById("dalee");
var btn2 = document.getElementById("skrit");

function buttonClicked() {


    document.getElementById("text").innerHTML = "Первый снег выпал – через сорок дней зима придет.<br> Первый снег выпал на сухую землю – жди хорошее лето<br/> Птицы хохлятся – жди холодную зиму.<br>\
 Птицы одновременно улетают – жди холодную зиму.<br>\
 Первые ночные заморозки приходят через три недели после отлета журавлей.<br>\
 Журавли летят на юг высоко, не быстро и курлычут – жди хорошую осень.<br>\
Перелетные птицы высоко летят – к снежной, теплой зиме, а низко перелетные птицы летят – зимой мало снега будет и зима холодная.<br>\
Скворцы все не улетают – осень будет сухая.<br>\
 Гром в сентябре – жди теплую осень и снежную зиму.<br>\
 Гром в октябре – к малоснежной зиме.<br>\
Осень дождлива – весна ясна да красна.<br>\
Осень ясная – весна мокрая.<br>\
Холодная осень – теплая весна.<br>\
Коли лето мокрое и осень теплая – жди затяжную зиму.<br>\
Осень коротка – зима долга.<br>\
Ранний снег осенью – к ранней весне.<br>\
Коли осенью снег ляжет споро – весной не сойдет долго.<br>\
Задержалась листва на деревьях – жди суровой, долгой зимы.<br>\
Рано опала листва с березы, клена и осины – жди раннюю весну.<br>\
Долгая золотая осень – морозы придут не скоро.<br>\
Поздний и короткий листопад – к крутой и долгой зиме.<br>\
Урожай рябины – осень дождливая, а зима строгая.<br>\
Много шелухи на луковицах – быть зиме холодной.<br>\
Шишки на ели выросли низко – быть ранним морозам, а коли высоко шишки на ели – настоящие холода придут в январе.<br>\
Коли осенью много орехов, а грибов не было – жди зиму суровую да снежную.<br>\
Луга опутаны паутиной – осень будет долгая и теплая.<br>\
Белка низко дупло выбрала – жди морозную зиму, белка выбрала дупло высоко – жди теплую зимы.<br>\
 Комары объявились поздней осенью – к теплой зиме.<br>\
Какова погода в ноябре – таков и май.<br> "


}

function buttonClicked2() {
    console.log(document.getElementById("text").innerHTML);
    document.getElementById("text").innerHTML = "";
}

btn.addEventListener("click", buttonClicked);
btn2.addEventListener("click", buttonClicked2);



