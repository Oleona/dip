var btn5 = document.getElementById("daleesu");
var btn6 = document.getElementById("skritsu");

function buttonClicked5() {


    document.getElementById("textsu").innerHTML = "Ясный и светлый Млечный путь – к теплой погоде.<br>\
    Сильное мерцание звезд на рассвете предвещает дождь через пару - тройку дней.<br>\
    Красные облака до восхода или на закате – к ветру.<br>\
    Долгий протяжный гром – к ненастью, краткий гром – к солнечной погоде.<br>\
    Дождь идет и слышатся далекие раскаты грома – к долгому дождю.<br>\
    Крупные дождевые пузыри на лужах – дождь усилится.<br>\
    Ветви елей и можжевельника приподнимаются – жди солнечную погоду.<br>\
    Листья деревьев поворачиваются обратной стороной – к скорому дождю.<br>\
    Цветы стали сильнее пахнуть – к дождю.<br>\
    Птицы громко поют – жди хорошую погоду.<br>\
    Птицы хохлятся – будет ненастье.<br>\
    Птицы ходят по лужам – жди дождя.<br>\
    Ласточки вечером высоко летают – завтра будет солнечно.<br>\
    Ласточки низко над землей летают – к дождю <br>\
    Петух поет днем – к дождю.<br>\
    Бросила рыба вдруг клевать – доброй погоде не бывать.<br>\
    Кузнечики стрекочут вечером громким хором – к хорошей погоде.<br>\
    Пауки пропали – к дождю, коли пауков видно много – жди хорошую погоду.<br>\
    Паук активно плетет сеть – будет малооблачно.<br>\
    Утренний туман стелется по воде – будет ясно.<br> "
}

function buttonClicked6() {
    console.log(document.getElementById("textsu").innerHTML);
    document.getElementById("textsu").innerHTML = "";
}

btn5.addEventListener("click", buttonClicked5);
btn6.addEventListener("click", buttonClicked6);
