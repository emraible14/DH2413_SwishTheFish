import type { ColorLike } from "color";

export interface FishConfig {
    tailId: number,
    bodyId: number,
    headId: number,
    color: ColorLike;
    deviceId: string | null;
}