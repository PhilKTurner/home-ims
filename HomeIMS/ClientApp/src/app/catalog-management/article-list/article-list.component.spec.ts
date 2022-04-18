import { DebugElement } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { Article } from '../models/article';
import { ArticleService } from '../services/article.service';

import { ArticleListComponent } from './article-list.component';

describe('ArticleListComponent', () => {
  let component: ArticleListComponent;
  let fixture: ComponentFixture<ArticleListComponent>;

  let getArticlesSpy: jasmine.Spy;

  let testArticles: Article[] = [{
    id: 42,
    ean: '123456789',
    description: 'TestArticle'
  }];

  beforeEach(async () => {
    const articleServiceMock = jasmine.createSpyObj('ArticleService', ['getArticles']);
    getArticlesSpy = articleServiceMock.getArticles.and.returnValue(of(testArticles));

    await TestBed.configureTestingModule({
      providers: [
        { provide: ArticleService, useValue: articleServiceMock }
      ],
      declarations: [ ArticleListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ArticleListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(fixture.componentInstance).toBeDefined();
  });

  // TODO DOM testing
  // it('', () => {
  //   const componentDebug: DebugElement = fixture.debugElement;
  //   const componentHtml: HTMLElement = componentDebug.nativeElement;
  //   const table = componentHtml.querySelector('table')!;
  // });
});
