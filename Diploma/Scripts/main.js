const button = document.querySelector('.buttonsearch');
const modalContainer = document.querySelector('.modal-container');
const closeBtn = document.querySelector('.close-btn');

// when the button is clicked, open the modal window
button.addEventListener('click', function() {
    modalContainer.classList.add('modal-is-open');  
});

// when the close button inside the modal is clicked, close the modal window
closeBtn.addEventListener('click', function() {
    modalContainer.classList.remove('modal-is-open');   
});