import {Injectable, Inject} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import { User } from '../models/user';
import { map } from 'rxjs/operators';
import { AuthenticationService } from './authentication.service';
import { Team } from '../models/team';
import { createOfflineCompileUrlResolver } from '@angular/compiler';

@Injectable({ providedIn: 'root' })
export class TeamService {
    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private authenticationService: AuthenticationService){}

    getCurrentMembers(teamId: number) {
        return (this.http.get<User[]>(`${this.baseUrl}api/teams/${teamId}/members`)
        .pipe(map(res => res.map(m => new User(m)))));
    }

    getTeam(teamId: number) {
        return (this.http.get<Team>(`${this.baseUrl}api/teams/${teamId}`)
        .pipe(map(res => res)));
    }

    addMember(Id: number, Action: String, MemberPseudo: String){
        return this.http.put<User>(`${this.baseUrl}api/teams/${Id}`, { Action, MemberPseudo }).pipe(map(user => new User(user)));
    }

    removeMember(Id: number, Action: String, MemberPseudo: String){
        return this.http.put<User>(`${this.baseUrl}api/teams/${Id}`, { Action, MemberPseudo }).pipe(map(user => new User(user)));
    }

    getAllTeams(){
        return this.http.get<Team[]>(`${this.baseUrl}api/teams`);
    }

    addTeam(Id: number, Name: String, MemberPseudo: String){
        return this.http.post<Team>(`${this.baseUrl}api/teams`, { Id, Name, MemberPseudo }).pipe(map(team => new Team(team)));
    }

    deleteTeam(Id: number){
        return this.http.delete(`${this.baseUrl}api/teams/${Id}`);
    }
}