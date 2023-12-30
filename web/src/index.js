const { SessionIdRequest, VoteRequest, GetChallengeRequest, AnswerChallengeRequest, GetImageRequest, GetVotedImagesRequest } = require('./memeoftheyear_pb.js');
const { VoteServicePromiseClient, ChallengeServicePromiseClient, ImageServicePromiseClient } = require('./memeoftheyear_grpc_web_pb.js');
const Cookies = require('js-cookie');

const host = process.env.GRPC_HOST;

const challengeService = new ChallengeServicePromiseClient(host);
const voteService = new VoteServicePromiseClient(host);
const imageService = new ImageServicePromiseClient(host);

module.exports = {
  setSessionId: function(session) {
    Cookies.set("session", session);
  },
  getSessionId: function() {
    return Cookies.get("session");
  },
  getChallenge: async function () {
    const request = new GetChallengeRequest();
    const response = await challengeService.getChallenge(request, {});

    return { "id": response.getQuestionid(), "text": response.getQuestiontext() };
  },
  answerChallenge: async function (id, answer) {
    const request = new AnswerChallengeRequest();
    request.setQuestionid(id);
    request.setAnswer(answer);

    const response = await challengeService.answerChallenge(request, {});

    return response.getSuccess();
  },
  init: async function () {
    const request = new SessionIdRequest();
    const response = await voteService.init(request, {});

    return { "sessionId": response.getSessionid(), "imageId": response.getImageid() };
  },
  
  like: async function (sessionId, imageId) {
    let request = new VoteRequest();
    request.setSessionid(sessionId);
    request.setImageid(imageId);

    const response = await voteService.like(request, {});
    return response.getNextimageid();
  },
  dislike: async function (sessionId, imageId) {
    let request = new VoteRequest();
    request.setSessionid(sessionId);
    request.setImageid(imageId);

    const response = await voteService.dislike(request, {});
    return response.getNextimageid();
  },
  skip: async function (sessionId, imageId) {
    let request = new VoteRequest();
    request.setSessionid(sessionId);
    request.setImageid(imageId);

    const response = await voteService.skip(request, {});
    return response.getNextimageid();
  },
  getImage: async function (imageId) {
    let request = new GetImageRequest();
    request.setImageid(imageId);

    const response = await imageService.getImage(request, {});

    return response.getImagecontent();
  },
  getMostedLikedImages: async function () {
    let request = new GetVotedImagesRequest();
    request.setCount(10);

    const response = await imageService.getMostLikedImages(request, {});

    return response.getEntriesList().map((v) => {
      return { "imageId": v.getImageid(), "likes": v.getVotes() }
    });
  }
};