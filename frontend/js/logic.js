const modal_element = document.getElementById("staticBackdrop")
const challenge_modal = new bootstrap.Modal(modal_element);

function disable_voting(remainingLikes, finished) {
    if(remainingLikes == 0) {
        const buttons = document.getElementsByClassName("btn-like");
        [...buttons].forEach((element) => element.setAttribute("disabled", ""));
    }
    
    if(finished) {
        const buttons = document.getElementsByClassName("btn");
        [...buttons].forEach((element) => element.setAttribute("disabled", ""));
    }
}

async function like_meme() {
    const image = document.getElementById("meme-img");
    const image_id = image.getAttribute("image-id");
    const session_id = MemeOfTheYear.getSessionId();

    const nextImage = await MemeOfTheYear.like(session_id, image_id);
    const content = await MemeOfTheYear.getImage(session_id, nextImage.imageId);

    image.src = content;
    image.setAttribute("image-id", nextImage.imageId);

    disable_voting(nextImage.likes, nextImage.finished);
}

async function dislike_meme() {
    const image = document.getElementById("meme-img");
    const image_id = image.getAttribute("image-id");
    const session_id = MemeOfTheYear.getSessionId();

    const nextImage = await MemeOfTheYear.dislike(session_id, image_id);
    const content = await MemeOfTheYear.getImage(session_id, nextImage.imageId);

    image.src = content;
    image.setAttribute("image-id", nextImage.imageId);

    disable_voting(nextImage.likes, nextImage.finished);
}

async function skip_meme() {
    const image = document.getElementById("meme-img");
    const image_id = image.getAttribute("image-id");
    const session_id = MemeOfTheYear.getSessionId();

    const nextImage = await MemeOfTheYear.skip(session_id, image_id);
    const content = await MemeOfTheYear.getImage(session_id, nextImage.imageId);

    image.src = content;
    image.setAttribute("image-id", nextImage.imageId);

    disable_voting(nextImage.likes, nextImage.finished);
}

async function check_challenge() {
    const input = document.getElementById("challenge-answer");
    const question_text_element = document.getElementById("question-text");

    const questionId = question_text_element.getAttribute("question-id");
    const answer = await MemeOfTheYear.answerChallenge(questionId, input.value);

    if (answer) {
        challenge_modal.hide();
        const session = await MemeOfTheYear.init();
        MemeOfTheYear.setSessionId(session.sessionId);

        const content = await MemeOfTheYear.getImage(session.sessionId, session.imageId);

        const image = document.getElementById("meme-img");
        image.src = content;
        image.setAttribute("image-id", session.imageId);
    }
}

async function init_this_stuff() {
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));

    let cookie_session = MemeOfTheYear.getSessionId();

    // show popup to prevent access of anybody
    const check = document.getElementById("check-challenge-button");
    const input = document.getElementById("challenge-answer");

    modal_element.addEventListener('shown.bs.modal', async () => {
        input.focus();

        const question = await MemeOfTheYear.getChallenge();
        const question_text_element = document.getElementById("question-text");
        question_text_element.setAttribute("question-id", question.id);
        question_text_element.textContent = question.text;
    })

    input.addEventListener("input", e => {
        if (input.value.length == 0) {
            check.setAttribute("disabled", "");
        } else {
            check.removeAttribute("disabled");
        }
    });

    if (cookie_session === "" || cookie_session === undefined) {
        challenge_modal.show();
    } else {
        const nextImage = await MemeOfTheYear.skip(cookie_session, "");
        const content = await MemeOfTheYear.getImage(cookie_session, nextImage.imageId);

        const image = document.getElementById("meme-img");
        image.src = content;
        image.setAttribute("image-id", nextImage.imageId);

        disable_voting(nextImage.likes, nextImage.finished);
    }
}