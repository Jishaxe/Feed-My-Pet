"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const index_1 = require("./index");
// const liveComment = new LiveComment({liveId: 'y-2mvg2GvSg'})
const liveComment = new index_1.LiveChat({ channelId: 'UCSFCh5NL4qXrAy9u-u2lX3g' });
liveComment.on('comment', (comment) => {
    console.log(comment.message);
});
liveComment.on('start', (liveId) => {
    console.log(`Start to watch stream ${liveId}`);
});
liveComment.on('end', () => {
    console.log('End stream.');
});
liveComment.on('error', (error) => {
    console.error(error);
});
liveComment.start();
//# sourceMappingURL=debug.js.map