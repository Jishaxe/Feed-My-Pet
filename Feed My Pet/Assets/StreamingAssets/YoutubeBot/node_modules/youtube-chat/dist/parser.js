"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function parseThumbnailToImageItem(data, alt) {
    const thumbnail = data.pop();
    if (thumbnail) {
        return {
            url: thumbnail.url,
            width: thumbnail.width,
            height: thumbnail.height,
            alt: alt,
        };
    }
    return;
}
function parseEmojiToImageItem(data) {
    return parseThumbnailToImageItem(data.emoji.image.thumbnails, data.emoji.shortcuts.shift());
}
function parseMessages(runs) {
    return runs.map((run) => {
        if ('text' in run) {
            return run;
        }
        else {
            return parseEmojiToImageItem(run);
        }
    });
}
function actionToRenderer(action) {
    if (!action.addChatItemAction) {
        return null;
    }
    const item = action.addChatItemAction.item;
    if (item.liveChatTextMessageRenderer) {
        return item.liveChatTextMessageRenderer;
    }
    else if (item.liveChatPaidMessageRenderer) {
        return item.liveChatPaidMessageRenderer;
    }
    else if (item.liveChatPaidStickerRenderer) {
        return item.liveChatPaidStickerRenderer;
    }
    else {
        return item.liveChatMembershipItemRenderer;
    }
}
exports.actionToRenderer = actionToRenderer;
function usecToTime(usec) {
    return Math.floor(Number(usec) / 1000);
}
exports.usecToTime = usecToTime;
function parseData(data) {
    const messageRenderer = actionToRenderer(data);
    if (messageRenderer === null) {
        return null;
    }
    let message = [];
    if ('message' in messageRenderer) {
        message = messageRenderer.message.runs;
    }
    else if ('headerSubtext' in messageRenderer) {
        message = messageRenderer.headerSubtext.runs;
    }
    const ret = {
        id: messageRenderer.id,
        author: {
            name: messageRenderer.authorName.simpleText,
            thumbnail: parseThumbnailToImageItem(messageRenderer.authorPhoto.thumbnails, messageRenderer.authorName.simpleText),
            channelId: messageRenderer.authorExternalChannelId,
        },
        message: parseMessages(message),
        membership: Boolean('headerSubtext' in messageRenderer),
        isOwner: false,
        timestamp: usecToTime(messageRenderer.timestampUsec),
    };
    if (messageRenderer.authorBadges) {
        const badge = messageRenderer.authorBadges[0].liveChatAuthorBadgeRenderer;
        if (badge.customThumbnail) {
            ret.author.badge = {
                thumbnail: parseThumbnailToImageItem(badge.customThumbnail.thumbnails, badge.tooltip),
                label: badge.tooltip,
            };
        }
        else {
            ret.isOwner = true;
        }
    }
    if ('sticker' in messageRenderer) {
        ret.superchat = {
            amount: messageRenderer.purchaseAmountText.simpleText,
            color: messageRenderer.backgroundColor,
            sticker: parseThumbnailToImageItem(messageRenderer.sticker.thumbnails, messageRenderer.sticker.accessibility.accessibilityData.label)
        };
    }
    else if ('purchaseAmountText' in messageRenderer) {
        ret.superchat = {
            amount: messageRenderer.purchaseAmountText.simpleText,
            color: messageRenderer.bodyBackgroundColor,
        };
    }
    return ret;
}
exports.parseData = parseData;
//# sourceMappingURL=parser.js.map