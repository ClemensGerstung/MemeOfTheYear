const { SessionIdRequest, VoteRequest, GetChallengeRequest, AnswerChallengeRequest, GetImageRequest, GetVotedImagesRequest, GetImagesRequest } = require('../protogen/memeoftheyear_pb.js');
const { VoteServicePromiseClient, ChallengeServicePromiseClient, ImageServicePromiseClient, ResultServicePromiseClient } = require('../protogen/memeoftheyear_grpc_web_pb.js');
const Cookies = require('js-cookie');

const host = process.env.GRPC_HOST;

const challengeService = new ChallengeServicePromiseClient(host);
const voteService = new VoteServicePromiseClient(host);
const imageService = new ImageServicePromiseClient(host);
const resultService = new ResultServicePromiseClient(host);

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
    request.setMaxlikes(4); // TODO: use env variable
    const response = await voteService.init(request, {});

    return { "sessionId": response.getSessionid(), "imageId": response.getImageid() };
  },
  
  like: async function (sessionId, imageId) {
    let request = new VoteRequest();
    request.setSessionid(sessionId);
    request.setImageid(imageId);

    const response = await voteService.like(request, {});
    return {
      "imageId": response.getNextimageid(),
      "likes": response.getRemaininglikes(),
      "finished": response.getFinished()
    };
  },
  dislike: async function (sessionId, imageId) {
    let request = new VoteRequest();
    request.setSessionid(sessionId);
    request.setImageid(imageId);

    const response = await voteService.dislike(request, {});
    return {
      "imageId": response.getNextimageid(),
      "likes": response.getRemaininglikes(),
      "finished": response.getFinished()
    };
  },
  skip: async function (sessionId, imageId) {
    let request = new VoteRequest();
    request.setSessionid(sessionId);
    request.setImageid(imageId);

    const response = await voteService.skip(request, {});
    return {
      "imageId": response.getNextimageid(),
      "likes": response.getRemaininglikes(),
      "finished": response.getFinished()
    };
  },
  getImage: async function (sessionId, imageId) {
    let request = new GetImageRequest();
    request.setImageid(imageId);
    request.setSessionid(sessionId);

    const response = await imageService.getImage(request, {});

    return response.getImagecontent();
  },
  getMostLikedImages: async function (sessionId) {
    let request = new GetVotedImagesRequest();
    request.setCount(10);
    request.setSessionid(sessionId);

    const response = await resultService.getMostLikedImages(request, {});

    return response.getEntriesList().map((v) => {
      return { "imageId": v.getImageid(), "likes": v.getVotes() }
    });
  },
  getMostDislikedImages: async function (sessionId) {
    let request = new GetVotedImagesRequest();
    request.setCount(10);
    request.setSessionid(sessionId);

    const response = await resultService.getMostDislikedImages(request, {});

    return response.getEntriesList().map((v) => {
      return { "imageId": v.getImageid(), "likes": v.getVotes() }
    });
  },
  getAllImages: async function (sessionId) {
    let request = new GetImagesRequest();
    request.setSessionid(sessionId);

    const response = await imageService.getAllImages(request, {});

    return response.getEntriesList().map((v) => {
      return { "imageId": v }
    });
  }
};