import { Section } from "./section";
import { User } from "./user";

export class Card {
    id: number;
    title: string;
    content: string;
    position: number;
    section: Section;
    author: User;
    usersIn: User[];

    constructor(data: any){
        this.id = data.id;
        this.title = data.title;
        this.content = data.content;
        this.position = data.position;
        this.section = data.section;
        this.author = data.author;
        this.usersIn = data.usersIn;
    }

}