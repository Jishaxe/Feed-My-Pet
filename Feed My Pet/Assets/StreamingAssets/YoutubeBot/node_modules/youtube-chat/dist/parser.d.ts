import { Action, LiveChatMembershipItemRenderer, LiveChatPaidMessageRenderer, LiveChatPaidStickerRenderer, LiveChatTextMessageRenderer } from './yt-response';
interface ImageItem {
    url: string;
    alt: string;
    width: number;
    height: number;
}
declare type MessageItem = {
    text: string;
} | ImageItem;
export interface CommentItem {
    id: string;
    author: {
        name: string;
        thumbnail?: ImageItem;
        channelId: string;
        badge?: {
            thumbnail: ImageItem;
            label: string;
        };
    };
    message: MessageItem[];
    superchat?: {
        amount: string;
        color: number;
        sticker?: ImageItem;
    };
    membership: boolean;
    isOwner: boolean;
    timestamp: number;
}
export declare function actionToRenderer(action: Action): LiveChatTextMessageRenderer | LiveChatPaidMessageRenderer | LiveChatPaidStickerRenderer | LiveChatMembershipItemRenderer | null;
export declare function usecToTime(usec: string): number;
export declare function parseData(data: Action): CommentItem | null;
export {};
