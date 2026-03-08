$(function () {
    "use strict";

    // Feather Icon Init Js
    feather.replace();

    $(".preloader").fadeOut();

    // this is for close icon when navigation open in mobile view
    $(".nav-toggler").on('click', function () {
        $("#main-wrapper").toggleClass("show-sidebar");
        $(".nav-toggler i").toggleClass("ti-menu");
    });

    // ==============================================================
    // Right sidebar options
    // ==============================================================
    $(function () {
        $(".service-panel-toggle").on('click', function () {
            $(".customizer").toggleClass('show-service-panel');

        });
        $('.page-wrapper').on('click', function () {
            $(".customizer").removeClass('show-service-panel');
        });
    });

    // ==============================================================
    //tooltip
    // ==============================================================
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })

    // ==============================================================
    //Popover
    // ==============================================================
    $(function () {
        $('[data-toggle="popover"]').popover()
    })

    // ==============================================================
    // Perfact scrollbar
    // ==============================================================
    $('.message-center, .customizer-body, .scrollable, .scroll-sidebar').perfectScrollbar({
        wheelPropagation: !0
    });

    // ==============================================================
    // Resize all elements
    // ==============================================================
    $("body, .page-wrapper").trigger("resize");
    $(".page-wrapper").delay(20).show();
    // ==============================================================
    // To do list
    // ==============================================================
    $(".list-task li label").click(function () {
        $(this).toggleClass("task-done");
    });

    // ==============================================================
    // This is for the innerleft sidebar
    // ==============================================================
    $(".show-left-part").on('click', function () {
        $('.left-part').toggleClass('show-panel');
        $('.show-left-part').toggleClass('ti-menu');
    });

    // For Custom File Input
    $('.custom-file-input').on('change', function () {
        //get the file name
        var fileName = $(this).val();
        //replace the "Choose a file" label
        $(this).next('.custom-file-label').html(fileName);
    })

    // datatable
    window.defaultDataTableSettings = {
        paging: true,
        pageLength: 5,
        responsive: true,
        layout: {
            topStart: {
                pageLength: {
                    menu: [5, 10, 15, 20]
                }
            },
            topEnd: {
                search: {
                    placeholder: ''
                }
            },
            bottomStart: 'info',
            bottomEnd: 'paging'
        }, search: {
            return: true
        }
    };

    window.initTinyMCE = function () {
        tinymce.remove(); // luôn remove trước khi init để tránh trùng instance
        tinymce.init({
            selector: 'textarea',
            max_height: 400,
            menubar: false,
            branding: false,
            plugins:
                'autoresize advlist autolink lists link image charmap print preview' +
                'anchor searchreplace visualblocks code fullscreen' +
                'insertdatetime image table paste code help wordcount',
            toolbar: 'undo redo | styles | ' +
                'bold italic backcolor | alignleft aligncenter ' +
                'alignright alignjustify | bullist numlist outdent indent | ' +
                'removeformat image',
            setup: (editor) => {
                editor.on('init', () => {
                    editor.getContainer().style.transition = 'border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out';
                });
                editor.on('focus', () => {
                    editor.getContainer().style.borderColor = 'rgba(0,0,0,.25)';
                });
                editor.on('blur', () => {
                    editor.getContainer().style.borderColor = '';
                });
            },
            content_style: 'body { font-family:Rubik,sans-serif; font-size:14px }',
            style_formats: [
                // Adds a h1 format to style_formats that applies a class of heading
                { title: 'Heading 1', block: 'h3', classes: 'heading' },
                { title: 'Heading 2', block: 'h4', classes: 'heading' }
            ],
            image_dimensions: false,
            image_caption: true,
            /* enable automatic uploads of images represented by blob or data URIs*/
            automatic_uploads: true,
            /*
              URL of our upload handler (for more details check: https://www.tiny.cloud/docs/configure/file-image-upload/#images_upload_url)
              images_upload_url: 'postAcceptor.php',
              here we add custom filepicker only to Image dialog
            */
            file_picker_types: 'image',
            /* and here's our custom image picker*/
            file_picker_callback: function (cb, value, meta) {
                var input = document.createElement('input');
                input.setAttribute('type', 'file');
                input.setAttribute('accept', 'image/*');

                /*
                  Note: In modern browsers input[type="file"] is functional without
                  even adding it to the DOM, but that might not be the case in some older
                  or quirky browsers like IE, so you might want to add it to the DOM
                  just in case, and visually hide it. And do not forget do remove it
                  once you do not need it anymore.
                */

                input.onchange = function () {
                    var file = this.files[0];

                    var reader = new FileReader();
                    reader.onload = function () {
                        /*
                          Note: Now we need to register the blob in TinyMCEs image blob
                          registry. In the next release this part hopefully won't be
                          necessary, as we are looking to handle it internally.
                        */
                        var id = 'blobid' + (new Date()).getTime();
                        var blobCache = tinymce.activeEditor.editorUpload.blobCache;
                        var base64 = reader.result.split(',')[1];
                        var blobInfo = blobCache.create(id, file, base64);
                        blobCache.add(blobInfo);

                        /* call the callback and populate the Title field with the file name */
                        cb(blobInfo.blobUri(), { title: file.name });
                    };
                    reader.readAsDataURL(file);
                };

                input.click();
            }
        });
    };

    // Gọi 1 lần khi load
    initTinyMCE();
});
