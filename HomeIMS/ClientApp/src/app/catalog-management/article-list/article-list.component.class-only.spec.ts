import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { Article } from '../models/article';
import { ArticleService } from '../services/article.service';

import { ArticleListComponent } from './article-list.component';

describe('ArticleListComponent - ClassOnly', () => {
  let component: ArticleListComponent;

  let getArticlesSpy: jasmine.Spy;

  let testArticles: Article[] = [{
    id: 42,
    ean: '123456789',
    description: 'TestArticle'
  }];

  beforeEach(() => {
    const articleServiceMock = jasmine.createSpyObj('ArticleService', ['getArticles']);
    getArticlesSpy = articleServiceMock.getArticles.and.returnValue(of(testArticles));

    TestBed.configureTestingModule({
      providers: [
        ArticleListComponent,
        { provide: ArticleService, useValue: articleServiceMock }
      ]
    });

    component = TestBed.inject(ArticleListComponent);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have no articles after construction', () => {
    expect(component.articles).toHaveSize(0);
  });

  it('should load articles on init', () => {
    component.ngOnInit();

    expect(getArticlesSpy.calls.count()).toBe(1);
    expect(component.articles).toHaveSize(testArticles.length);
  });
});
