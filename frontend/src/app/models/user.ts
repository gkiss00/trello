import { Card } from "./card";
import { Organization } from "./organization";
import { Table } from "./table";
import { Team } from "./team";

export enum Role {
    Member = 0,
    Manager = 1,
    Admin = 2
  }

export class User {

    id: number;
    pseudo : string;
    password: string;
    firstName: string ;
    lastName: string ;
    birthDate: string;
    role: Role;
    token: string;
    teamsIn: Team[];
    organizationsIn: Organization[];
    tablesOwned: Table[];
    tablesIn: Table[];
    cardsIn: Card[];

    constructor(data?: any){
        if(data){
            this.id = data.id;
            this.pseudo = data.pseudo;
            this.password = data.password;
            this.firstName = data.firsName;
            this.lastName = data.lastName;
            this.role = data.role || Role.Member;
            this.token = data.token;
            this.birthDate = data.birthDate&&
            data.birthDate.length > 10 ? data.birthDate.substring(0, 10) : data.birthDate;
            this.teamsIn = data.teamsIn;
            this.organizationsIn = data.organizationsIn;
            this.tablesOwned = data.tablesOwned;
            this.tablesIn = data.tablesIn;
            this.cardsIn = data.cardsIn;
        }
    }

    public get roleAsString(): string {
        return Role[this.role];
    }
}