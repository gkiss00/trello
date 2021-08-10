import { Section } from "./section";
import { Team } from "./team";
import { User } from "./user";

export enum View {
    Public = 0,
    Organization = 1,
    Team = 2,
    Private = 3,
}

export class Table {
    id: number;
    title: string;
    view: View;
    team: Team;
    teamId?: number;
    organizationId? : number;
    usersIn?: User[];
    usersInString? : String[];
    owner: User;
    sections: Section[];

    constructor(data?: any){
        if(data){
            this.id = data.id;
            this.title = data.title;
            this.view = data.view;
            this.team = data.team;
            this.teamId = data.teamId;
            this.owner = data.owner;
            this.sections = data.sections;
            this.usersIn = data.usersIn;
        }
    }

    
}