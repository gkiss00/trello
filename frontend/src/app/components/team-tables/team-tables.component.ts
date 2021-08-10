import { Component} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Team } from 'src/app/models/team';
import { UserService } from 'src/app/services/user.service';
import { Router } from '@angular/router';

@Component({
    templateUrl: 'team-tables.component.html',
    styleUrls: ['team-tables.component.css']
})
export class TeamTablesComponent{
    team : Team;
    teamId : number;

    constructor(
        private router: Router,
        private userService : UserService,
        private activatedRoute : ActivatedRoute
    ) {
        this.teamId = parseInt(this.activatedRoute.snapshot.paramMap.get('id'));
        userService.getCurrentUserTeam(this.teamId).subscribe(
            team => {
                this.team = team;
            },
            error => {
                this.router.navigate(['/unknown']);
        });
    }

}