import { Component, ElementRef, HostListener, ViewChild, ViewChildren} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Table } from 'src/app/models/table';
import { SectionService } from 'src/app/services/section.service';
import { TableService } from 'src/app/services/table.service';
import { UserService } from 'src/app/services/user.service';
import { Card } from 'src/app/models/card';
import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { CardService } from 'src/app/services/card.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { User } from 'src/app/models/user';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { Section } from 'src/app/models/section';

@Component({
    templateUrl: 'table-cards.component.html',
    styleUrls: ['table-cards.component.css'],
})

export class TableCardsComponent{

    table: Table;
    selectedId : number;
    currentCardSectionId: number;
    selectedSection: number;
    hidden: boolean;
    cardEdit: boolean = false;
    membersInput: boolean = false;
    leaveTable: boolean = false;
    sectionTitleEdit: boolean = false;
    cardTitleError: boolean = false;
    eraseSection: boolean[];
    editSection: boolean[];
    currentCard: Card;
    sectionEditValue: string;
    users: Observable<User[]>;
    @ViewChildren('newCard') el: Array<ElementRef>;
    @ViewChild('newMember') newMemberWindow: ElementRef;
    @ViewChildren('sectionErase') sectionEraseWindow: Array<ElementRef>;
    @ViewChildren('sectionEdit') sectionEditWindow: Array<ElementRef>;
    private leaveWindow: ElementRef;
    @ViewChild('leave', { static: false }) set content(content :ElementRef){
        if(content){
            this.leaveWindow = content;
        }
    }

    constructor(
        private router: Router,
        private userService : UserService,
        private tableService: TableService,
        private cardService: CardService,
        private sectionService: SectionService,
        private activeRoute : ActivatedRoute,
        private authenticationService: AuthenticationService
    ) {
        this.hidden = false;
        this.selectedId = -1;
        let tableId = +this.activeRoute.snapshot.paramMap.get('id');
        this.userService.getCurrentUserTableContent(tableId).subscribe(
            table => {
                this.table = table;
                this.eraseSection = new Array(table.sections.length);
                this.editSection = new Array(table.sections.length);
            },
            error => {
                this.router.navigate(['/unknown']);
        });
    }

    private searchUser(value: string){
        if(value != ''){
            value = this.normalizeValue(value);
            this.userService.getUsersByKeywords(value).subscribe(users => {
                this.users = of(users);
            });
        }
    }

    private normalizeValue(value: string): string {
        return value.toLowerCase().replace(/\s/g, '');
    }

    get currentUser() {
        return this.authenticationService.currentUser;
    }

    private addSection(title: string){
        if(title && title != ""){
            this.tableService.addSection(title, this.table.id).subscribe(s => {
                this.table.sections.push(s);
            });
        }
    }

    private addCard(title: string, sectionId: number){
        if(title && title != ""){
            let section = this.table.sections.find(s => s.id == sectionId);
            this.sectionService.addCard(title, section.id, section.cards.length, this.currentUser.id).subscribe(c =>{
                    this.table.sections.find(s => s.id == sectionId).cards.push(c);
                    this.cardTitleError = false;
                },
                error => {
                    this.cardTitleError = true;
            });
        }
    }

    private displayUsername(user: User): string {
        return user.pseudo;
    }

    private addMember(user: User){
        this.resetUsers();
        this.tableService.addMemberToTable(this.table.id, user.id).subscribe(u => {
            let tmp: User = new User();
            tmp.id = user.id;
            tmp.pseudo = user.pseudo;
            this.table.usersIn.push(tmp);
        });
    }

    private tableUsers(): User[]{
        let tmp: User[] = new Array();
        tmp = tmp.concat(this.table.usersIn);
        tmp.push(this.table.owner);
        tmp.sort((u1, u2) => (u1.pseudo > u2.pseudo) ? 1 : (u1.pseudo < u2.pseudo) ? -1 : 0);
        return tmp;
    }

    private tableUsersNoOwner(): User[]{
        let tmp: User[] = new Array();
        tmp = tmp.concat(this.table.usersIn);
        tmp.push(this.table.owner);
        tmp.sort((u1, u2) => (u1.pseudo > u2.pseudo) ? 1 : (u1.pseudo < u2.pseudo) ? -1 : 0);
        return tmp;
    }

    private AlreadyJoined(userId: number): boolean{
        return (userId == this.currentUser.id || userId == this.table.owner.id || this.table.usersIn.includes(this.table.usersIn.find(u => u.id == userId)));
    }

    private resetUsers(){
        this.users = new Observable<User[]>();
    }

    private editCard(card: Card, sectionId: number){
        this.currentCard = card;
        this.currentCardSectionId = sectionId;
    }

    private close(request: boolean){
        this.currentCard = null;
        this.currentCardSectionId = -1;
        this.cardEdit = request;
    }

    private cardUpdate(card: Card){
        if(card != null){
            this.currentCard.title = card.title;
            this.currentCard.usersIn = new Array();
            card.usersIn.forEach(u => {
                this.currentCard.usersIn.push(u);
            });
        } else {
            let tmpSection = this.table.sections.find(s => s.id == this.currentCardSectionId);
            const index = tmpSection.cards.findIndex(c => c.id == this.currentCard.id);
            tmpSection.cards.splice(index, 1);
            this.close(false);
        }
    }

    private reducePseudo(pseudo: string): string {
        return pseudo.substr(0, 2).toUpperCase();
    }

    private removeMember(user: User){
        this.tableService.removeMember(this.table.id, user.id).subscribe(u => {
            const index = this.table.usersIn.findIndex(u => u.id == user.id);
            this.table.usersIn.splice(index, 1);
            this.removeMemberFromCards(user);
        });
    }

    private stopPropagation(event){
        event.stopPropagation();
    }

    private removeMemberFromCards(user: User){
        this.table.sections.forEach(s => {
            s.cards.forEach(c =>{
                const index = c.usersIn.findIndex(u => u.id == user.id);
                c.usersIn.splice(index, 1);
            });
        });
    }

    private leaveBoard(): void{
        this.tableService.removeMember(this.table.id, this.currentUser.id).subscribe(u => {
            const index = this.table.usersIn.findIndex(u => u.id == this.currentUser.id);
            this.table.usersIn.splice(index, 1);
            this.removeMemberFromCards(this.currentUser);
            this.router.navigate(["/table"]);
        });
    }

    private removeBoard(): void{
        this.tableService.deleteTable(this.table.id).subscribe(t => {
            this.router.navigate(["/table"]);
        });
    }

    private removeSection(sectionId: number): void{
        this.sectionService.deleteSection(sectionId).subscribe(s => {
            const index = this.table.sections.findIndex(s => s.id == sectionId);
            this.table.sections.splice(index, 1);
        });
    }

    private closeEraseWindows(index: number):void{
        for(let i = 0; i < this.eraseSection.length; i++){
            if(i != index)
                this.eraseSection[i] = false;
        }
    }

    private closeEditWindows(index: number):void{
        for(let i = 0; i < this.editSection.length; i++){
            if(i != index)
                this.editSection[i] = false;
        }
    }

    private editSectionTitle(sectionId: number){
        if(this.sectionEditValue != null && this.sectionEditValue.trim() !== "" )
        this.sectionService.updateSectionTitle(sectionId, this.sectionEditValue).subscribe(s =>{
            var tmp = this.table.sections.find(s => s.id == sectionId);
            tmp.title = this.sectionEditValue;
            this.sectionEditValue = "";
            this.selectedSection = -1;
        });
    }

    @HostListener('document:click', ['$event.target'])
    public onClick(targetElement) {
        if(this.el.filter(e => e.nativeElement.contains(targetElement)).length == 0){
            this.selectedId = -1;
            this.hidden = false;
            this.cardTitleError = false;
        } if(!this.leaveWindow.nativeElement.contains(targetElement)) {
            this.leaveTable = false;
        } if(!this.newMemberWindow.nativeElement.contains(targetElement)) {
            this.membersInput = false;
        } if(this.sectionEraseWindow.filter(e => e.nativeElement.contains(targetElement)).length == 0) {
            this.eraseSection = this.eraseSection.map(b => b = false);
        } if(this.sectionEditWindow.filter(e => e.nativeElement.contains(targetElement)).length == 0) {
            this.editSection = this.editSection.map(b => b = false);
            if(this.sectionTitleEdit)
                this.editSectionTitle(this.selectedSection);
        } 
    }

    drop(event: CdkDragDrop<Card[]>) {
        if (event.previousContainer === event.container) {
            this.cardService.updateCardPosition(event.item.data[0], event.currentIndex).subscribe();
            moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
        } else {
            let sectionDifference = parseInt(event.previousContainer.id.replace("cdk-drop-list-", "")) - parseInt(event.container.id.replace("cdk-drop-list-", ""));
            let sectionId = event.item.data[1] - sectionDifference;
            sectionId = this.table.sections[sectionId].id;
            this.cardService.updateCardSection(sectionId, event.item.data[0], event.currentIndex).subscribe();
            transferArrayItem(event.previousContainer.data,
                            event.container.data,
                            event.previousIndex,
                            event.currentIndex);
        }
    }

}