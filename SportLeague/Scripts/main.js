$("#Poster").change(function () {
    readURL(this);
});

var readURL = function (input) {

    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#poster-prew').attr('src', e.target.result);
        }

        reader.readAsDataURL(input.files[0]);
    }
}

$('.pagination .disabled a, .pagination .active a').on('click', function (e) {
        e.preventDefault();
});
