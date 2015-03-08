// Client-side cheatsheet operations.
var CS = { };

CS.getBoxX = function(index) {
    return (CS.boxWidth + CS.betweenBoxSpace) * index;
}

CS.getBoxCenter = function(index) {
    return CS.getBoxX(index) + (CS.boxWidth / 2);
}

CS.drawBox = function(index) {
    var svgns = "http://www.w3.org/2000/svg",
        boxX = CS.getBoxX(index),
        labelX = CS.getBoxCenter(index), 
        box = document.createElementNS(svgns, "rect"),
        label = document.createElementNS(svgns, "text"),
        textNode = document.createTextNode(CS.labels[index]);

    box.setAttribute("id", "box" + index);
    box.setAttribute("x", boxX);
    box.setAttribute("y", 0);
    box.setAttribute("rx", 3);
    box.setAttribute("ry", 3);
    box.setAttribute("width", CS.boxWidth);
    box.setAttribute("height", CS.svgHeight);
    box.setAttribute("class", "box boxoff");
    // The prepend is to get the z-order right w.r.t. the arrow.
    CS.$svg.prepend(box);
    CS.boxes[index] = box;

    label.setAttribute("id", "boxlabel" + index);
    label.setAttribute("x", labelX);
    label.setAttribute("y", 20);
    label.setAttribute("text-anchor", "middle");
    label.setAttribute("class", "boxlabel");
    label.appendChild(textNode);
    CS.$svg.append(label);
};

CS.drawBoxes = function() {
    for (var i = 0; i < 5; i++) {
        CS.drawBox(i);
    }
}

CS.mouseEnter = function(e) {
    var $target = $(e.target),
        fgColor = $target.css("color"),
        $section = $target.closest(".section"),
        bgColor = $section.css("backgroundColor");

    $target.css({ color: bgColor, backgroundColor: fgColor });
    return false;
}

CS.mouseLeave = function(e) {
    var $target = $(e.target);
    $target.css({ color: "", backgroundColor: "" });
    return false;
}

CS.whatToIndex = function(what) {
    switch (what) {
        case "S": return 0;
        case "W": return 1;
        case "I": return 2;
        case "L": return 3;
        case "R": return 4;
    }
}

CS.turnBoxOff = function(index) {
    CS.boxes[index].setAttribute("class", "boxoff");
}

CS.turnBoxOn = function(index) {
    CS.boxes[index].setAttribute("class", "boxon");
}

CS.setMarkers = function(on) {
    var m = on ? "url(#arrowhead)" : "";
    $("#theline").attr("marker-start", m).attr("marker-end", m);
}

CS.hideArrow = function() {
    $("#theline").attr("y1", "-999").attr("y2", "-999");
}

CS.drawArrow = function(idx1, idx2) {
    var center1 = CS.getBoxCenter(idx1),
        center2 = CS.getBoxCenter(idx2);
    
    // Adjust the arrow point a bit to account for the triangle.
    if (center2 > center1) {
        // arrow points from left to right.
        center2 -= 30;
    }
    else {
        // arrow points from right to left.
        center2 += 30;
    }

    $("#theline").attr("x1", center1).attr("y1", CS.arrowY).
                  attr("x2", center2).attr("y2", CS.arrowY);
    CS.setMarkers(true);
}

CS.drawBlock = function(idx1, idx2) {
    var left,
        right,
        center;

    if (!idx2) {
        // Just draw over 1 box.
        center = CS.getBoxCenter(idx1);
        left = center - CS.boxWidth * 0.3;
        right = center + CS.boxWidth * 0.3;
    }
    else {
        // Draw between two boxes.
        left = CS.getBoxCenter(idx1);
        right = CS.getBoxCenter(idx2);
    }

    $("#theline").attr("x1", left).attr("y1", CS.arrowY).
                  attr("x2", right).attr("y2", CS.arrowY);
    CS.setMarkers(false);
}

CS.handleClick = function(e) {
    var $target = $(e.target),
        what = $target.data("what"),
        hint = $target.data("hint"),
        url = $target.data("url"),
        i,
        t = $target.text(),
        r1 = /^\d.*\. /,
        r2 = /.*,NG.*/;

    if (what.match(r2)) {
        // "No git prefix" is specified.
        what = what.replace(",NG", "");
    }
    else {
        // We want a git prefix, but where? Some cheats are numbered lists,
        // we have to put the 'git' after the number or it looks a mess.
        if (t.match(r1)) {
            t = t.replace(r1, "$& git ");
        } 
        else {
            t = "git " + t;
        }
    }

    if (url && url !== "#") {
        t += "&nbsp;<a id=\"ancMore\" href=\"" + url + "\">" + url + "</a>";
    }
    $("#headingHint").html(t);
    $("#spanHint").text(hint);

    // Cancel all box highlights.
    for (i = 0; i < CS.boxes.length; i++) {
        CS.turnBoxOff(i);
    }
    CS.hideArrow();

    console.log("what = " + what);
    if (!what) {
        return;
    }


    var lastChar = what[what.length - 1],
        upperBound = lastChar === "A" ? what.length - 1 : what.length,
        idx;

    for (i = 0; i < upperBound; i++) {
        idx = CS.whatToIndex(what[i]);
        CS.turnBoxOn(idx);
    }


    idx = CS.whatToIndex(what[0]);
    if (what.length === 1) {
        CS.drawBlock(idx);
    }
    else {
        idx2 = CS.whatToIndex(what[upperBound - 1]);

        if (lastChar === "A") {
            CS.drawArrow(idx, idx2);
        }
        else {
            CS.drawBlock(idx, idx2);
        }
    }

    return false;
};

CS.setTheme = function (themeId) {
    // Prevent problems with cookies from the old style of theme handling hanging around.
    if (!themeId || themeId.length <= 3)
        themeId = "theme.solarized_dark.css";
    var $cssLink = $("#themeSheet");
    $cssLink.attr("href", "content/" + themeId);
    $.cookie("gcstheme", themeId, { expires: 3 * 365 });
}

$(function() {
    // mouseEnter/mouseLeave events don't bubble.
    $(".section dt").hover(CS.mouseEnter, CS.mouseLeave);
    // but clicks do.
    $(".section dt").click(CS.handleClick);

    // A click on the "extras" menu is handled by the colorbox plugin.
    $("#cboxLink").colorbox({inline:true, width:"50%", title:"Credits, etc."});

    $("#themeSelector").change(function(e) {
        CS.setTheme($(e.target).val());
    });

    // Restore last used theme, ensuring the select box matches the cookie.
    //console.log("cookie = " + document.cookie);
    var lastTheme = $.cookie("gcstheme");
    if (lastTheme) {
        CS.setTheme(lastTheme);
        $("#themeSelector").val(lastTheme);
    }

    CS.boxes = [];
    CS.labels = ["Stash", "Workspace", "Index", "Local", "Remote"];
    // We have to wait until #newBoxes exists before working these things out.
    CS.$svg = $("#newBoxes");   
    CS.svgWidth = CS.$svg.width();
    CS.svgHeight = CS.$svg.height();
    // We need to fit 5 boxes and 4 inner spaces into the svgWidth.
    CS.betweenBoxSpace = CS.svgWidth >= 640 ? 10 : 5,
    CS.boxWidth = (CS.svgWidth - (CS.betweenBoxSpace * 4)) / 5;
    CS.arrowY = (CS.svgHeight / 2) + 10;

    CS.drawBoxes();
});
