// ==============================================================
// Auto select left navbar
// ==============================================================
$(function () {
    "use strict";
    var currentUrl = window.location.pathname; // Lấy đường dẫn hiện tại (path)
    var basePath = currentUrl.split("/").slice(0, -1).join("/"); // Lấy base path (loại bỏ ID hoặc action cuối)

    var element = $("ul#sidebarnav a").filter(function () {
        var hrefPath = new URL(this.href, window.location.origin).pathname; // Chuyển href sang path đầy đủ
        return hrefPath.startsWith(basePath); // So sánh base path (không tính tham số động)
    });

    element.parentsUntil(".sidebar-nav").each(function (index) {
        if ($(this).is("li") && $(this).children("a").length !== 0) {
            $(this).children("a").addClass("active");
            $(this).parent("ul#sidebarnav").length === 0
                ? $(this).addClass("active")
                : $(this).addClass("selected");
        } else if (!$(this).is("ul") && $(this).children("a").length === 0) {
            $(this).addClass("selected");
        } else if ($(this).is("ul")) {
            $(this).addClass("in");
        }
    });

    element.addClass("active");
    $("#sidebarnav a").on("click", function (e) {
        if (!$(this).hasClass("active")) {
            // hide any open menus and remove all other classes
            $("ul", $(this).parents("ul:first")).removeClass("in");
            $("a", $(this).parents("ul:first")).removeClass("active");

            // open our new menu and add the open class
            $(this).next("ul").addClass("in");
            $(this).addClass("active");
        } else if ($(this).hasClass("active")) {
            $(this).removeClass("active");
            $(this).parents("ul:first").removeClass("active");
            $(this).next("ul").removeClass("in");
        }
    });
    $("#sidebarnav >li >a.has-arrow").on("click", function (e) {
        e.preventDefault();
    });
});
