/**
 * @fileoverview gRPC-Web generated client stub for 
 * @enhanceable
 * @public
 */

// Code generated by protoc-gen-grpc-web. DO NOT EDIT.
// versions:
// 	protoc-gen-grpc-web v1.5.0
// 	protoc              v3.15.8
// source: memeoftheyear.proto


/* eslint-disable */
// @ts-nocheck



const grpc = {};
grpc.web = require('grpc-web');

const proto = require('./memeoftheyear_pb.js');

/**
 * @param {string} hostname
 * @param {?Object} credentials
 * @param {?grpc.web.ClientOptions} options
 * @constructor
 * @struct
 * @final
 */
proto.VoteServiceClient =
    function(hostname, credentials, options) {
  if (!options) options = {};
  options.format = 'text';

  /**
   * @private @const {!grpc.web.GrpcWebClientBase} The client
   */
  this.client_ = new grpc.web.GrpcWebClientBase(options);

  /**
   * @private @const {string} The hostname
   */
  this.hostname_ = hostname.replace(/\/+$/, '');

};


/**
 * @param {string} hostname
 * @param {?Object} credentials
 * @param {?grpc.web.ClientOptions} options
 * @constructor
 * @struct
 * @final
 */
proto.VoteServicePromiseClient =
    function(hostname, credentials, options) {
  if (!options) options = {};
  options.format = 'text';

  /**
   * @private @const {!grpc.web.GrpcWebClientBase} The client
   */
  this.client_ = new grpc.web.GrpcWebClientBase(options);

  /**
   * @private @const {string} The hostname
   */
  this.hostname_ = hostname.replace(/\/+$/, '');

};


/**
 * @const
 * @type {!grpc.web.MethodDescriptor<
 *   !proto.SessionIdRequest,
 *   !proto.SessionIdResponse>}
 */
const methodDescriptor_VoteService_Init = new grpc.web.MethodDescriptor(
  '/VoteService/Init',
  grpc.web.MethodType.UNARY,
  proto.SessionIdRequest,
  proto.SessionIdResponse,
  /**
   * @param {!proto.SessionIdRequest} request
   * @return {!Uint8Array}
   */
  function(request) {
    return request.serializeBinary();
  },
  proto.SessionIdResponse.deserializeBinary
);


/**
 * @param {!proto.SessionIdRequest} request The
 *     request proto
 * @param {?Object<string, string>} metadata User defined
 *     call metadata
 * @param {function(?grpc.web.RpcError, ?proto.SessionIdResponse)}
 *     callback The callback function(error, response)
 * @return {!grpc.web.ClientReadableStream<!proto.SessionIdResponse>|undefined}
 *     The XHR Node Readable Stream
 */
proto.VoteServiceClient.prototype.init =
    function(request, metadata, callback) {
  return this.client_.rpcCall(this.hostname_ +
      '/VoteService/Init',
      request,
      metadata || {},
      methodDescriptor_VoteService_Init,
      callback);
};


/**
 * @param {!proto.SessionIdRequest} request The
 *     request proto
 * @param {?Object<string, string>=} metadata User defined
 *     call metadata
 * @return {!Promise<!proto.SessionIdResponse>}
 *     Promise that resolves to the response
 */
proto.VoteServicePromiseClient.prototype.init =
    function(request, metadata) {
  return this.client_.unaryCall(this.hostname_ +
      '/VoteService/Init',
      request,
      metadata || {},
      methodDescriptor_VoteService_Init);
};


/**
 * @const
 * @type {!grpc.web.MethodDescriptor<
 *   !proto.VoteRequest,
 *   !proto.VoteResponse>}
 */
const methodDescriptor_VoteService_Like = new grpc.web.MethodDescriptor(
  '/VoteService/Like',
  grpc.web.MethodType.UNARY,
  proto.VoteRequest,
  proto.VoteResponse,
  /**
   * @param {!proto.VoteRequest} request
   * @return {!Uint8Array}
   */
  function(request) {
    return request.serializeBinary();
  },
  proto.VoteResponse.deserializeBinary
);


/**
 * @param {!proto.VoteRequest} request The
 *     request proto
 * @param {?Object<string, string>} metadata User defined
 *     call metadata
 * @param {function(?grpc.web.RpcError, ?proto.VoteResponse)}
 *     callback The callback function(error, response)
 * @return {!grpc.web.ClientReadableStream<!proto.VoteResponse>|undefined}
 *     The XHR Node Readable Stream
 */
proto.VoteServiceClient.prototype.like =
    function(request, metadata, callback) {
  return this.client_.rpcCall(this.hostname_ +
      '/VoteService/Like',
      request,
      metadata || {},
      methodDescriptor_VoteService_Like,
      callback);
};


/**
 * @param {!proto.VoteRequest} request The
 *     request proto
 * @param {?Object<string, string>=} metadata User defined
 *     call metadata
 * @return {!Promise<!proto.VoteResponse>}
 *     Promise that resolves to the response
 */
proto.VoteServicePromiseClient.prototype.like =
    function(request, metadata) {
  return this.client_.unaryCall(this.hostname_ +
      '/VoteService/Like',
      request,
      metadata || {},
      methodDescriptor_VoteService_Like);
};


/**
 * @const
 * @type {!grpc.web.MethodDescriptor<
 *   !proto.VoteRequest,
 *   !proto.VoteResponse>}
 */
const methodDescriptor_VoteService_Dislike = new grpc.web.MethodDescriptor(
  '/VoteService/Dislike',
  grpc.web.MethodType.UNARY,
  proto.VoteRequest,
  proto.VoteResponse,
  /**
   * @param {!proto.VoteRequest} request
   * @return {!Uint8Array}
   */
  function(request) {
    return request.serializeBinary();
  },
  proto.VoteResponse.deserializeBinary
);


/**
 * @param {!proto.VoteRequest} request The
 *     request proto
 * @param {?Object<string, string>} metadata User defined
 *     call metadata
 * @param {function(?grpc.web.RpcError, ?proto.VoteResponse)}
 *     callback The callback function(error, response)
 * @return {!grpc.web.ClientReadableStream<!proto.VoteResponse>|undefined}
 *     The XHR Node Readable Stream
 */
proto.VoteServiceClient.prototype.dislike =
    function(request, metadata, callback) {
  return this.client_.rpcCall(this.hostname_ +
      '/VoteService/Dislike',
      request,
      metadata || {},
      methodDescriptor_VoteService_Dislike,
      callback);
};


/**
 * @param {!proto.VoteRequest} request The
 *     request proto
 * @param {?Object<string, string>=} metadata User defined
 *     call metadata
 * @return {!Promise<!proto.VoteResponse>}
 *     Promise that resolves to the response
 */
proto.VoteServicePromiseClient.prototype.dislike =
    function(request, metadata) {
  return this.client_.unaryCall(this.hostname_ +
      '/VoteService/Dislike',
      request,
      metadata || {},
      methodDescriptor_VoteService_Dislike);
};


/**
 * @const
 * @type {!grpc.web.MethodDescriptor<
 *   !proto.VoteRequest,
 *   !proto.VoteResponse>}
 */
const methodDescriptor_VoteService_Skip = new grpc.web.MethodDescriptor(
  '/VoteService/Skip',
  grpc.web.MethodType.UNARY,
  proto.VoteRequest,
  proto.VoteResponse,
  /**
   * @param {!proto.VoteRequest} request
   * @return {!Uint8Array}
   */
  function(request) {
    return request.serializeBinary();
  },
  proto.VoteResponse.deserializeBinary
);


/**
 * @param {!proto.VoteRequest} request The
 *     request proto
 * @param {?Object<string, string>} metadata User defined
 *     call metadata
 * @param {function(?grpc.web.RpcError, ?proto.VoteResponse)}
 *     callback The callback function(error, response)
 * @return {!grpc.web.ClientReadableStream<!proto.VoteResponse>|undefined}
 *     The XHR Node Readable Stream
 */
proto.VoteServiceClient.prototype.skip =
    function(request, metadata, callback) {
  return this.client_.rpcCall(this.hostname_ +
      '/VoteService/Skip',
      request,
      metadata || {},
      methodDescriptor_VoteService_Skip,
      callback);
};


/**
 * @param {!proto.VoteRequest} request The
 *     request proto
 * @param {?Object<string, string>=} metadata User defined
 *     call metadata
 * @return {!Promise<!proto.VoteResponse>}
 *     Promise that resolves to the response
 */
proto.VoteServicePromiseClient.prototype.skip =
    function(request, metadata) {
  return this.client_.unaryCall(this.hostname_ +
      '/VoteService/Skip',
      request,
      metadata || {},
      methodDescriptor_VoteService_Skip);
};


/**
 * @param {string} hostname
 * @param {?Object} credentials
 * @param {?grpc.web.ClientOptions} options
 * @constructor
 * @struct
 * @final
 */
proto.ChallengeServiceClient =
    function(hostname, credentials, options) {
  if (!options) options = {};
  options.format = 'text';

  /**
   * @private @const {!grpc.web.GrpcWebClientBase} The client
   */
  this.client_ = new grpc.web.GrpcWebClientBase(options);

  /**
   * @private @const {string} The hostname
   */
  this.hostname_ = hostname.replace(/\/+$/, '');

};


/**
 * @param {string} hostname
 * @param {?Object} credentials
 * @param {?grpc.web.ClientOptions} options
 * @constructor
 * @struct
 * @final
 */
proto.ChallengeServicePromiseClient =
    function(hostname, credentials, options) {
  if (!options) options = {};
  options.format = 'text';

  /**
   * @private @const {!grpc.web.GrpcWebClientBase} The client
   */
  this.client_ = new grpc.web.GrpcWebClientBase(options);

  /**
   * @private @const {string} The hostname
   */
  this.hostname_ = hostname.replace(/\/+$/, '');

};


/**
 * @const
 * @type {!grpc.web.MethodDescriptor<
 *   !proto.GetChallengeRequest,
 *   !proto.GetChallengeResponse>}
 */
const methodDescriptor_ChallengeService_GetChallenge = new grpc.web.MethodDescriptor(
  '/ChallengeService/GetChallenge',
  grpc.web.MethodType.UNARY,
  proto.GetChallengeRequest,
  proto.GetChallengeResponse,
  /**
   * @param {!proto.GetChallengeRequest} request
   * @return {!Uint8Array}
   */
  function(request) {
    return request.serializeBinary();
  },
  proto.GetChallengeResponse.deserializeBinary
);


/**
 * @param {!proto.GetChallengeRequest} request The
 *     request proto
 * @param {?Object<string, string>} metadata User defined
 *     call metadata
 * @param {function(?grpc.web.RpcError, ?proto.GetChallengeResponse)}
 *     callback The callback function(error, response)
 * @return {!grpc.web.ClientReadableStream<!proto.GetChallengeResponse>|undefined}
 *     The XHR Node Readable Stream
 */
proto.ChallengeServiceClient.prototype.getChallenge =
    function(request, metadata, callback) {
  return this.client_.rpcCall(this.hostname_ +
      '/ChallengeService/GetChallenge',
      request,
      metadata || {},
      methodDescriptor_ChallengeService_GetChallenge,
      callback);
};


/**
 * @param {!proto.GetChallengeRequest} request The
 *     request proto
 * @param {?Object<string, string>=} metadata User defined
 *     call metadata
 * @return {!Promise<!proto.GetChallengeResponse>}
 *     Promise that resolves to the response
 */
proto.ChallengeServicePromiseClient.prototype.getChallenge =
    function(request, metadata) {
  return this.client_.unaryCall(this.hostname_ +
      '/ChallengeService/GetChallenge',
      request,
      metadata || {},
      methodDescriptor_ChallengeService_GetChallenge);
};


/**
 * @const
 * @type {!grpc.web.MethodDescriptor<
 *   !proto.AnswerChallengeRequest,
 *   !proto.AnswerChallengeResponse>}
 */
const methodDescriptor_ChallengeService_AnswerChallenge = new grpc.web.MethodDescriptor(
  '/ChallengeService/AnswerChallenge',
  grpc.web.MethodType.UNARY,
  proto.AnswerChallengeRequest,
  proto.AnswerChallengeResponse,
  /**
   * @param {!proto.AnswerChallengeRequest} request
   * @return {!Uint8Array}
   */
  function(request) {
    return request.serializeBinary();
  },
  proto.AnswerChallengeResponse.deserializeBinary
);


/**
 * @param {!proto.AnswerChallengeRequest} request The
 *     request proto
 * @param {?Object<string, string>} metadata User defined
 *     call metadata
 * @param {function(?grpc.web.RpcError, ?proto.AnswerChallengeResponse)}
 *     callback The callback function(error, response)
 * @return {!grpc.web.ClientReadableStream<!proto.AnswerChallengeResponse>|undefined}
 *     The XHR Node Readable Stream
 */
proto.ChallengeServiceClient.prototype.answerChallenge =
    function(request, metadata, callback) {
  return this.client_.rpcCall(this.hostname_ +
      '/ChallengeService/AnswerChallenge',
      request,
      metadata || {},
      methodDescriptor_ChallengeService_AnswerChallenge,
      callback);
};


/**
 * @param {!proto.AnswerChallengeRequest} request The
 *     request proto
 * @param {?Object<string, string>=} metadata User defined
 *     call metadata
 * @return {!Promise<!proto.AnswerChallengeResponse>}
 *     Promise that resolves to the response
 */
proto.ChallengeServicePromiseClient.prototype.answerChallenge =
    function(request, metadata) {
  return this.client_.unaryCall(this.hostname_ +
      '/ChallengeService/AnswerChallenge',
      request,
      metadata || {},
      methodDescriptor_ChallengeService_AnswerChallenge);
};


/**
 * @param {string} hostname
 * @param {?Object} credentials
 * @param {?grpc.web.ClientOptions} options
 * @constructor
 * @struct
 * @final
 */
proto.ImageServiceClient =
    function(hostname, credentials, options) {
  if (!options) options = {};
  options.format = 'text';

  /**
   * @private @const {!grpc.web.GrpcWebClientBase} The client
   */
  this.client_ = new grpc.web.GrpcWebClientBase(options);

  /**
   * @private @const {string} The hostname
   */
  this.hostname_ = hostname.replace(/\/+$/, '');

};


/**
 * @param {string} hostname
 * @param {?Object} credentials
 * @param {?grpc.web.ClientOptions} options
 * @constructor
 * @struct
 * @final
 */
proto.ImageServicePromiseClient =
    function(hostname, credentials, options) {
  if (!options) options = {};
  options.format = 'text';

  /**
   * @private @const {!grpc.web.GrpcWebClientBase} The client
   */
  this.client_ = new grpc.web.GrpcWebClientBase(options);

  /**
   * @private @const {string} The hostname
   */
  this.hostname_ = hostname.replace(/\/+$/, '');

};


/**
 * @const
 * @type {!grpc.web.MethodDescriptor<
 *   !proto.GetImageRequest,
 *   !proto.GetImageResponse>}
 */
const methodDescriptor_ImageService_GetImage = new grpc.web.MethodDescriptor(
  '/ImageService/GetImage',
  grpc.web.MethodType.UNARY,
  proto.GetImageRequest,
  proto.GetImageResponse,
  /**
   * @param {!proto.GetImageRequest} request
   * @return {!Uint8Array}
   */
  function(request) {
    return request.serializeBinary();
  },
  proto.GetImageResponse.deserializeBinary
);


/**
 * @param {!proto.GetImageRequest} request The
 *     request proto
 * @param {?Object<string, string>} metadata User defined
 *     call metadata
 * @param {function(?grpc.web.RpcError, ?proto.GetImageResponse)}
 *     callback The callback function(error, response)
 * @return {!grpc.web.ClientReadableStream<!proto.GetImageResponse>|undefined}
 *     The XHR Node Readable Stream
 */
proto.ImageServiceClient.prototype.getImage =
    function(request, metadata, callback) {
  return this.client_.rpcCall(this.hostname_ +
      '/ImageService/GetImage',
      request,
      metadata || {},
      methodDescriptor_ImageService_GetImage,
      callback);
};


/**
 * @param {!proto.GetImageRequest} request The
 *     request proto
 * @param {?Object<string, string>=} metadata User defined
 *     call metadata
 * @return {!Promise<!proto.GetImageResponse>}
 *     Promise that resolves to the response
 */
proto.ImageServicePromiseClient.prototype.getImage =
    function(request, metadata) {
  return this.client_.unaryCall(this.hostname_ +
      '/ImageService/GetImage',
      request,
      metadata || {},
      methodDescriptor_ImageService_GetImage);
};


/**
 * @const
 * @type {!grpc.web.MethodDescriptor<
 *   !proto.GetVotedImagesRequest,
 *   !proto.GetVotedImagesResponse>}
 */
const methodDescriptor_ImageService_GetMostLikedImages = new grpc.web.MethodDescriptor(
  '/ImageService/GetMostLikedImages',
  grpc.web.MethodType.UNARY,
  proto.GetVotedImagesRequest,
  proto.GetVotedImagesResponse,
  /**
   * @param {!proto.GetVotedImagesRequest} request
   * @return {!Uint8Array}
   */
  function(request) {
    return request.serializeBinary();
  },
  proto.GetVotedImagesResponse.deserializeBinary
);


/**
 * @param {!proto.GetVotedImagesRequest} request The
 *     request proto
 * @param {?Object<string, string>} metadata User defined
 *     call metadata
 * @param {function(?grpc.web.RpcError, ?proto.GetVotedImagesResponse)}
 *     callback The callback function(error, response)
 * @return {!grpc.web.ClientReadableStream<!proto.GetVotedImagesResponse>|undefined}
 *     The XHR Node Readable Stream
 */
proto.ImageServiceClient.prototype.getMostLikedImages =
    function(request, metadata, callback) {
  return this.client_.rpcCall(this.hostname_ +
      '/ImageService/GetMostLikedImages',
      request,
      metadata || {},
      methodDescriptor_ImageService_GetMostLikedImages,
      callback);
};


/**
 * @param {!proto.GetVotedImagesRequest} request The
 *     request proto
 * @param {?Object<string, string>=} metadata User defined
 *     call metadata
 * @return {!Promise<!proto.GetVotedImagesResponse>}
 *     Promise that resolves to the response
 */
proto.ImageServicePromiseClient.prototype.getMostLikedImages =
    function(request, metadata) {
  return this.client_.unaryCall(this.hostname_ +
      '/ImageService/GetMostLikedImages',
      request,
      metadata || {},
      methodDescriptor_ImageService_GetMostLikedImages);
};


/**
 * @const
 * @type {!grpc.web.MethodDescriptor<
 *   !proto.GetVotedImagesRequest,
 *   !proto.GetVotedImagesResponse>}
 */
const methodDescriptor_ImageService_GetMostDislikedImages = new grpc.web.MethodDescriptor(
  '/ImageService/GetMostDislikedImages',
  grpc.web.MethodType.UNARY,
  proto.GetVotedImagesRequest,
  proto.GetVotedImagesResponse,
  /**
   * @param {!proto.GetVotedImagesRequest} request
   * @return {!Uint8Array}
   */
  function(request) {
    return request.serializeBinary();
  },
  proto.GetVotedImagesResponse.deserializeBinary
);


/**
 * @param {!proto.GetVotedImagesRequest} request The
 *     request proto
 * @param {?Object<string, string>} metadata User defined
 *     call metadata
 * @param {function(?grpc.web.RpcError, ?proto.GetVotedImagesResponse)}
 *     callback The callback function(error, response)
 * @return {!grpc.web.ClientReadableStream<!proto.GetVotedImagesResponse>|undefined}
 *     The XHR Node Readable Stream
 */
proto.ImageServiceClient.prototype.getMostDislikedImages =
    function(request, metadata, callback) {
  return this.client_.rpcCall(this.hostname_ +
      '/ImageService/GetMostDislikedImages',
      request,
      metadata || {},
      methodDescriptor_ImageService_GetMostDislikedImages,
      callback);
};


/**
 * @param {!proto.GetVotedImagesRequest} request The
 *     request proto
 * @param {?Object<string, string>=} metadata User defined
 *     call metadata
 * @return {!Promise<!proto.GetVotedImagesResponse>}
 *     Promise that resolves to the response
 */
proto.ImageServicePromiseClient.prototype.getMostDislikedImages =
    function(request, metadata) {
  return this.client_.unaryCall(this.hostname_ +
      '/ImageService/GetMostDislikedImages',
      request,
      metadata || {},
      methodDescriptor_ImageService_GetMostDislikedImages);
};


/**
 * @const
 * @type {!grpc.web.MethodDescriptor<
 *   !proto.GetVotedImagesRequest,
 *   !proto.GetVotedImagesResponse>}
 */
const methodDescriptor_ImageService_GetMostSkippedImages = new grpc.web.MethodDescriptor(
  '/ImageService/GetMostSkippedImages',
  grpc.web.MethodType.UNARY,
  proto.GetVotedImagesRequest,
  proto.GetVotedImagesResponse,
  /**
   * @param {!proto.GetVotedImagesRequest} request
   * @return {!Uint8Array}
   */
  function(request) {
    return request.serializeBinary();
  },
  proto.GetVotedImagesResponse.deserializeBinary
);


/**
 * @param {!proto.GetVotedImagesRequest} request The
 *     request proto
 * @param {?Object<string, string>} metadata User defined
 *     call metadata
 * @param {function(?grpc.web.RpcError, ?proto.GetVotedImagesResponse)}
 *     callback The callback function(error, response)
 * @return {!grpc.web.ClientReadableStream<!proto.GetVotedImagesResponse>|undefined}
 *     The XHR Node Readable Stream
 */
proto.ImageServiceClient.prototype.getMostSkippedImages =
    function(request, metadata, callback) {
  return this.client_.rpcCall(this.hostname_ +
      '/ImageService/GetMostSkippedImages',
      request,
      metadata || {},
      methodDescriptor_ImageService_GetMostSkippedImages,
      callback);
};


/**
 * @param {!proto.GetVotedImagesRequest} request The
 *     request proto
 * @param {?Object<string, string>=} metadata User defined
 *     call metadata
 * @return {!Promise<!proto.GetVotedImagesResponse>}
 *     Promise that resolves to the response
 */
proto.ImageServicePromiseClient.prototype.getMostSkippedImages =
    function(request, metadata) {
  return this.client_.unaryCall(this.hostname_ +
      '/ImageService/GetMostSkippedImages',
      request,
      metadata || {},
      methodDescriptor_ImageService_GetMostSkippedImages);
};


module.exports = proto;

