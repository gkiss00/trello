import { Input } from '@angular/core';
import { OnInit } from '@angular/core';
import { Component} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Team } from 'src/app/models/team';
import { UserService } from 'src/app/services/user.service';

@Component({
    selector: 'team-table',
    templateUrl: 'single-team-table.component.html',
    styleUrls: ['single-team-table.component.css']
})
export class SingleTeamTableComponent implements OnInit {
    @Input() currentTeam : Team;
    @Input() fromOrga : boolean;

    constructor(
        private router: Router,
        private activeRoute: ActivatedRoute,
        private userService: UserService
    ) { }

    ngOnInit(): void {
        this.activeRoute.params.subscribe(routeParams => {
            let teamId;
            if (this.currentTeam.id > 0 && this.fromOrga == true)
                teamId = this.currentTeam.id;
            else
                teamId = +this.activeRoute.snapshot.paramMap.get('id');
            if(teamId > 0)
                this.userService.getCurrentUserTeam(teamId).subscribe(team => {
                        this.currentTeam = team;
                    },
                    error => {
                        this.router.navigate(['/unknown']);
                });
        });
    }

    titleUrlConverter(title: string): string{
        return title.replace(/\s/g, "-");
    }

}