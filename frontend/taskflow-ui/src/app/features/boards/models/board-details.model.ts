import { BoardColumn } from "./board-column.model";

export interface BoardDetails{
    id: string;
    name: string;
    projectId: string;
    column: BoardColumn[];
}