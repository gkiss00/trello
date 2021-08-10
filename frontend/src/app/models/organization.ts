import { Team } from "./team";
import { User } from "./user";

export class Organization{
    id: number;
    name: string;
    usersIn: User[];
    userList: string[];
    teams: Team[];

    constructor(data: any){
        this.id = data.id;
        this.name = data.name;
        this.userList = data.UseList;
        this.usersIn = data.usersIn;
        this.teams = data.teams;
    }
}