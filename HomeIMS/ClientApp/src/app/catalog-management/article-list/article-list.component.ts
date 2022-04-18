import { Component, OnInit } from '@angular/core';
import { Article } from '../models/article';
import { ArticleService } from '../services/article.service';

@Component({
  selector: 'hims-article-list',
  templateUrl: './article-list.component.html',
  styleUrls: ['./article-list.component.css']
})
export class ArticleListComponent implements OnInit {
  public articles: Article[] = [];

  public displayedColumns: string[] = ['id', 'ean', 'description'];

  constructor(private articleService: ArticleService) { }

  ngOnInit(): void {
    this.loadArticles();
  }

  public loadArticles(): void {
    this.articleService.getArticles()
        .subscribe(articles => this.articles = articles);
  }
}
