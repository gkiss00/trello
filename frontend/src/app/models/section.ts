import { Card } from "./card";
import { Table } from "./table";

export class Section {
    id: number;
    title: string;
    table: Table;
    tableId: number;
    cards: Card[];

    constructor(data: any){
        this.id = data.id;
        this.title = data.title;
        this.table = data.table;
        this.tableId = data.tableId;
        this.cards = data.cards;
    }
}