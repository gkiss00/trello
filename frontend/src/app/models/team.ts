import { Organization } from "./organization";
import { Table } from "./table";
import { User } from "./user";

export class Team{
    id: number;
    name: string;
    usersIn: User[];
    tablesTeam : Table[];
    organization: Organization;
    organizationId? : number;

    constructor(data: any){
        this.id = data.id;
        this.name = data.name;
        this.usersIn = data.usersIn;
        this.tablesTeam = data.tablesTeam;
        this.organization = data.organization;
        this.organizationId = data.organizationId;
    }
}