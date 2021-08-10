import {Injectable, Inject} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import { User } from '../models/user';
import { map } from 'rxjs/operators';
import { AuthenticationService } from './authentication.service';
import { Table } from '../models/table';
import { Team } from '../models/team';
import { Organization } from '../models/organization';

@Injectable({ providedIn: 'root' })
export class UserService {
    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private authenticationService: AuthenticationService){}

    getCurrentUserTables() {
        return (this.http.get<Table[]>(`${this.baseUrl}api/tables/user/${this.authenticationService.currentUser.id}`)
        .pipe(map(res => res.map(m => new Table(m)))));
    }

    getCurrentUserTableContent(tableId : number){
        return (this.http.get<Table>(`${this.baseUrl}api/tables/user/table/${tableId}`)
        .pipe(map(res => res)));
    }

    getCurrentUserTeams() {
        return (this.http.get<Team[]>(`${this.baseUrl}api/teams/user/${this.authenticationService.currentUser.id}/teams`)
        .pipe(map(res => res.map(m => new Team(m)))));
    }

    getCurrentUserTeamsWithOrga() {
        return (this.http.get<Team[]>(`${this.baseUrl}api/teams/user/${this.authenticationService.currentUser.id}/teams/orga`)
        .pipe(map(res => res.map(m => new Team(m)))));
    }

    getCurrentUserTeamsTables() {
        return (this.http.get<Team[]>(`${this.baseUrl}api/teams/user/${this.authenticationService.currentUser.id}`)
        .pipe(map(res => res.map(m => new Team(m)))));
    }

    getCurrentUserTeam(teamId : number) {
        return (this.http.get<Team>(`${this.baseUrl}api/teams/team/${teamId}`)
        .pipe(map(res => res)));
    }

    getCurrentUserOrga(orgId : number) {
        return (this.http.get<Team[]>(`${this.baseUrl}api/teams/orga/${orgId}`)
        .pipe(map(res => res.map(m => new Team(m)))));
    }


    getCurrentUserOrganizations() {
        return (this.http.get<Organization[]>(`${this.baseUrl}api/organizations/user/${this.authenticationService.currentUser.id}/organizations`)
        .pipe(map(res => res.map(m => new Organization(m)))));
    }

    getAllUsers() {
        return (this.http.get<User[]>(`${this.baseUrl}api/users`)
        .pipe(map(res => res.map(u => new User(u)))));
    }

    getUsersByKeywords(keywords: string){
        return this.http.get<User[]>(`${this.baseUrl}api/users/byName/${keywords}`).pipe(map(res => res.map(u => new User(u))));
    }
}