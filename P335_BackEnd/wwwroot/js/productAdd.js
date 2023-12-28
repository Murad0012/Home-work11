const imgInput = document.querySelector('.img-input');
const imgPreview = document.querySelector('.img-preview');

imgInput.addEventListener('change', (e) => {
    let img = e.target.files[0]
    let blobUrl = URL.createObjectURL(img)
    imgPreview.setAttribute('src', blobUrl)
})