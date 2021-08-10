import {Observable} from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';
import {map, startWith} from 'rxjs/operators';
import { Component, Input, OnInit} from '@angular/core';
import { FormControlName, FormGroup } from '@angular/forms';
import { FormControl } from '@angular/forms';
import { FormBuilder } from '@angular/forms';
import { Validators } from '@angular/forms';
import { forEach } from 'lodash-es';
import { Organization } from 'src/app/models/organization';
import { Table } from 'src/app/models/table';
import { Team } from 'src/app/models/team';
import { User } from 'src/app/models/user';
import { UserService } from 'src/app/services/user.service';
import { TableService } from 'src/app/services/table.service';
import { AuthenticationService } from 'src/app/services/authentication.service';


@Component({
    selector: 'create-table',
    templateUrl: 'create-table.component.html',
    styleUrls: ['create-table.component.css']
})

export class CreateTableComponent implements OnInit {
    public frm: FormGroup;
    loading = false;
    submitted = false;
    public ctlTitle: FormControl;
    public ctlView: FormControl;
    public ctlTeam: FormControl;
    public ctlOrganization: FormControl;
    public selectedView: Number;
    public userTeams: Team[] = [];
    public userOrganizations: Organization[] = [];
    public usersIn : User[] = [];
    public usersInToDisplay : string[] = [];
    public allUsers : User[] = [];

    private checked : boolean;
    public userNotExist : boolean = false;
    public userAlreadyIn : boolean = false;
    public selfAdded : boolean = false;
    public checkedButUsersInEmpty : boolean = false;
    public viewOneButTeamNull : boolean = false;
    public viewTwoButOrgaNull : boolean = false;
    public titleNotValid : boolean = false;
    public viewUnselected : boolean = false;

    public ctlUsersIn: FormControl;
    filteredOptions: Observable<User[]>;
  
    ngOnInit() {
    }
    
    constructor(private fb: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private userService: UserService,
        private tableService: TableService,
        private authenticationService: AuthenticationService
    ) {
        this.checked = false;
        this.selectedView = -1;
        this.userService.getCurrentUserTeams().subscribe(teams => {
            this.userTeams = teams;
        });
        this.userService.getCurrentUserOrganizations().subscribe(organizations => {
            this.userOrganizations = organizations;
        });
        this.userService.getAllUsers().subscribe(users => {
            this.allUsers = users;
        });
        this.allUsers.forEach(function (u) {
            this.usersToDisplay.includes(u.pseudo);
        });
        this.ctlTitle = this.fb.control('', [
            Validators.required,
            Validators.minLength(3),
            Validators.maxLength(20)
        ]);
        this.ctlView = this.fb.control('', [Validators.required]);
        this.ctlTeam = this.fb.control('', []);
        this.ctlOrganization = this.fb.control('', []);
        this.ctlUsersIn = this.fb.control('', []);

        this.frm = this.fb.group({
            title: this.ctlTitle,
            view: this.ctlView,
            team: this.ctlTeam,
            organization: this.ctlOrganization,
            usersIn: this.ctlUsersIn,
        });

    }

    get f() {return this.frm.controls;}

    viewChanged(event : any) {
        this.viewUnselected = false;
        this.checked = false;
        this.viewOneButTeamNull = false;
        this.viewTwoButOrgaNull = false;
        this.selectedView = event;
    }

    titleUrlConverter(title: string): string{
        return title.replace(/\s/g, "-");
    }

    create() {
        this.submitted = true;

        if (this.f.title.errors) {
            this.titleNotValid = true;
            return;
        }
        else if (this.f.view.errors) {
            this.viewUnselected = true;
            return;
        }
        else if (this.f.view.value == 0 && this.checked && this.usersIn.length == 0)
        {
            this.checkedButUsersInEmpty = true;
            return;
        }
        if (this.f.view.value == 1 && this.f.team.value == 0) {
            this.viewOneButTeamNull = true;
            return;
        }
        if (this.f.view.value == 2 && this.f.organization.value == 0) {
            this.viewTwoButOrgaNull = true;
            return;
        }
        
        this.loading = true;
        this.tableService.create(this.f.title.value, this.f.view.value, this.f.team.value,
            this.f.organization.value, this.usersInToDisplay)
            .subscribe(
                t => {
                    this.router.navigate(["/table/" + t.id + "/" + this.titleUrlConverter(this.f.title.value)]);
                },
                error => {
                    const errors = error.error.errors;
                    for (let field in errors) {
                        this.frm.get(field.toLowerCase()).setErrors({ custom: errors[field] })
                    }
                    this.loading = false;
                }
            );
    }    
    
    get currentUser() {
        return this.authenticationService.currentUser;
    }

    checkedChanged() {
        if (this.checked)
            this.checked = false;
        else
            this.checked = true;

    }

    addUser() {
        this.checkedButUsersInEmpty = false;
        this.userAlreadyIn = false;
        this.userNotExist = false;
        this.selfAdded = false;
        var input = this.ctlUsersIn.value;
        this.ctlUsersIn.setValue('');
        var user = this.allUsers.find(u => u.pseudo===input);
        if (input === this.currentUser.pseudo)
        {
            this.selfAdded = true;
        }
        else if (user) {
            this.usersIn.push(user);
            this.usersInToDisplay.push(user.pseudo);
            const index = this.allUsers.indexOf(user, 0);
            this.allUsers.splice(index, 1);
        }
        else {
            user = this.usersIn.find(u => u.pseudo===input);
            if (user)
                this.userAlreadyIn = true;
            else
                this.userNotExist = true;
        }
    }

    deleteUser(pseudo) {
        var user = this.usersIn.find(u => u.pseudo===pseudo);
        var index = this.usersIn.indexOf(user, 0);
        this.usersIn.splice(index, 1);
        index = this.usersInToDisplay.indexOf(pseudo, 0);
        this.usersInToDisplay.splice(index, 1);
        this.allUsers.push(user);
    }
    
    teamSelected() {
        this.viewOneButTeamNull = false;
    }

    orgaSelected() {
        this.viewTwoButOrgaNull = false;
    }

    titleChanged() {
        this.titleNotValid = false;
    }
}