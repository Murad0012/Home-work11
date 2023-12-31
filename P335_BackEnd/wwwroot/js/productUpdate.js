const imgInput = document.querySelector('.img-input');
const imgPreview = document.querySelector('.img-preview');
const imgDelete = document.querySelector(".delete-img")
const imgUrlInput = document.querySelector(".img-url-input")

imgInput.addEventListener('change', (e) => {
    let img = e.target.files[0]
    let blobUrl = URL.createObjectURL(img)
    imgPreview.setAttribute('src', blobUrl)
})

imgDelete.addEventListener('click', () => {
    imgUrlInput.setAttribute('value',null)
    imgPreview.setAttribute('src', "")
    imgDelete.remove()
})