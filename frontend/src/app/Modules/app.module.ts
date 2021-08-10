import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppRoutes } from '../routing/app.routing';

import { AppComponent } from '../components/app/app.component';
import { NavMenuComponent } from '../components/nav-menu/nav-menu.component';
import { LeftSidebarComponent } from '../components/left-sidebar/left-sidebar.component';
import { SingleTeamTableComponent } from '../components/single-team-table/single-team-table.component'
import { HomeComponent } from '../components/home/home.component';
import { JwtInterceptor } from '../interceptors/jwt.interceptor';
import { LoginComponent } from '../components/login/login.component';
import { TeamTablesComponent } from '../components/team-tables/team-tables.component';
import { SignupComponent } from '../components/signup/signup.component';
import { TableComponent } from '../components/table/table.component';
import { CreateTableComponent } from '../components/create-table/create-table.component';
import { MembersComponent } from '../components/members/members.component';
import { CardEditComponent} from '../components/card-edit/card-edit.component';
import { CardEditMemberAddComponent } from '../components/card-edit-member-add/card-edit-member-add.component';
import { TableCardsComponent } from '../components/table-cards/table-cards.component';
import { TableMemberRemoveComponent } from '../components/table-member-remove/table-member-remove.component';
import { UnknownComponent } from '../components/unknown/unknown.component';
import { RestrictedComponent } from '../components/restricted/restricted.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { SharedModule } from './shared.module';
import { SetFocusDirective } from '../directives/setfocus.directive';

import { OrganizationMembersComponent} from '../components/organization-members/organization-members.component';
import { CreateOrganizationComponent} from '../components/create-organization/create-organization.component';
import { DeleteUserOrgaComponent} from '../components/delete-userorga/delete-userorga.component';
import { DeleteOrganizationComponent} from '../components/delete-organization/delete-organization.component';
import { OrganizationDisplayTeamsComponent} from '../components/organization-displayteams/organization-displayteams.component';
import { OrganizationRemoveTeamComponent} from '../components/delete-teamorga/delete-teamorga.component';
import { SingleOrgaTableComponent} from '../components/single-orga-table/single-orga-table.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    TableComponent,
    CreateTableComponent,
    LoginComponent,
    SignupComponent,
    LeftSidebarComponent,
    SingleTeamTableComponent,
    TeamTablesComponent,
    TableCardsComponent,
    TableMemberRemoveComponent,
    CardEditComponent,
    CardEditMemberAddComponent,
    HomeComponent,
    UnknownComponent,
    RestrictedComponent,
    SetFocusDirective,
    MembersComponent,
    OrganizationMembersComponent,
    CreateOrganizationComponent,
    DeleteUserOrgaComponent,
    DeleteOrganizationComponent,
    OrganizationDisplayTeamsComponent,
    OrganizationRemoveTeamComponent,
    SingleOrgaTableComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutes,
    BrowserAnimationsModule,
    SharedModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }